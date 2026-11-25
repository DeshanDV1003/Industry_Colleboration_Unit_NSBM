using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSBM.ICU.Application.Common.Email;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Domain.Entities.Notifications;
using NSBM.ICU.Domain.Rules;
using NSBM.ICU.Infrastructure.Persistence;

namespace NSBM.ICU.Infrastructure.Email;

/// <summary>
/// Sends e-mail in the background, so no page ever waits for a mail server. The notifications table is the outbox:
/// only notifications that were committed get e-mailed, each person's new ones go out as one message, and a failed
/// send is retried on the next cycle. Once a week it also sends the digest.
/// </summary>
public sealed class EmailDispatcher(IServiceScopeFactory scopes, IOptions<EmailOptions> options, ILogger<EmailDispatcher> logger) : BackgroundService
{
    private const int BatchSize = 500;
    private const int DigestItems = 10;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (options.Value.Mode == EmailMode.Off)
        {
            logger.LogInformation("E-mail is off; notifications stay in the app.");
            return;
        }

        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(Math.Max(5, options.Value.DispatchIntervalSeconds)));
        do
        {
            try
            {
                await using var scope = scopes.CreateAsyncScope();
                var cycle = new Cycle(scope.ServiceProvider, options.Value, logger);
                await cycle.SendUpdatesAsync(stoppingToken);
                await cycle.SendWeeklyDigestsIfDueAsync(stoppingToken);
            }
            catch (Exception e) when (e is not OperationCanceledException)
            {
                logger.LogError(e, "The e-mail cycle failed; it will run again.");
            }
        }
        while (await timer.WaitForNextTickAsync(stoppingToken));
    }

    private sealed class Cycle(IServiceProvider services, EmailOptions options, ILogger logger)
    {
        private readonly ApplicationDbContext _db = services.GetRequiredService<ApplicationDbContext>();
        private readonly IEmailSender _sender = services.GetRequiredService<IEmailSender>();
        private readonly DateTime _now = services.GetRequiredService<IDateTime>().UtcNow;

        private sealed record Person(string Id, string Email, string Name, bool IsActive);

        public async Task SendUpdatesAsync(CancellationToken ct)
        {
            var pending = await _db.Notifications.Where(n => n.EmailHandledAt == null)
                .OrderBy(n => n.CreatedAt).Take(BatchSize).ToListAsync(ct);
            if (pending.Count == 0) return;

            var recipientIds = pending.Select(n => n.RecipientUserId).Distinct().ToList();
            var people = await PeopleAsync(recipientIds, ct);
            var preferences = await _db.EmailPreferences.Where(p => recipientIds.Contains(p.UserId)).ToDictionaryAsync(p => p.UserId, ct);
            var oldest = _now.AddHours(-options.MaxAgeHours);

            foreach (var group in pending.GroupBy(n => n.RecipientUserId))
            {
                var wantsEmails = !preferences.TryGetValue(group.Key, out var preference) || preference.NotificationsByEmail;
                var send = people.TryGetValue(group.Key, out var person) && person.IsActive
                    ? group.Where(n => n.CreatedAt >= oldest && EmailPolicy.ShouldEmail(n.Kind, wantsEmails)).ToList()
                    : [];

                if (send.Count > 0)
                {
                    var items = send.Select(n => new EmailItem(n.Title, n.Body, n.Link)).ToList();
                    try
                    {
                        await _sender.SendAsync(EmailTemplates.Updates(person!.Email, person.Name, items, options.AppBaseUrl), ct);
                    }
                    catch (Exception e) when (e is not OperationCanceledException)
                    {
                        // Left unhandled, so the next cycle retries this person's e-mail.
                        logger.LogWarning(e, "E-mail to {UserId} failed; retrying next cycle.", group.Key);
                        continue;
                    }
                }

                // Sent, or deliberately not e-mailed (choice, kind, inactive account, too old): never looked at again.
                foreach (var notification in group) notification.MarkEmailHandled(_now);
            }

            await _db.SaveChangesAsync(ct);
        }

        public async Task SendWeeklyDigestsIfDueAsync(CancellationToken ct)
        {
            var zone = TimeZoneInfo.FindSystemTimeZoneById(options.TimeZone);
            var local = TimeZoneInfo.ConvertTimeFromUtc(_now, zone);
            if (local.DayOfWeek != options.DigestDay || local.Hour < options.DigestHour) return;

            // The start of today's digest window, in UTC: a digest sent after it means this week is done.
            var windowStart = TimeZoneInfo.ConvertTimeToUtc(local.Date.AddHours(options.DigestHour), zone);
            var people = await PeopleAsync(null, ct);
            var preferences = await _db.EmailPreferences.ToDictionaryAsync(p => p.UserId, ct);

            foreach (var person in people.Values.Where(p => p.IsActive))
            {
                preferences.TryGetValue(person.Id, out var preference);
                if (preference is { WeeklyDigestByEmail: false } || preference?.LastDigestSentAt >= windowStart) continue;

                var unread = await _db.Notifications.AsNoTracking()
                    .Where(n => n.RecipientUserId == person.Id && n.ReadAt == null)
                    .OrderByDescending(n => n.CreatedAt).Take(DigestItems)
                    .Select(n => new EmailItem(n.Title, n.Body, n.Link)).ToListAsync(ct);
                try
                {
                    await _sender.SendAsync(EmailTemplates.WeeklyDigest(person.Email, person.Name, unread, options.AppBaseUrl), ct);
                }
                catch (Exception e) when (e is not OperationCanceledException)
                {
                    logger.LogWarning(e, "Weekly digest to {UserId} failed; retrying next cycle.", person.Id);
                    continue;
                }

                if (preference is null)
                {
                    preference = new EmailPreference { UserId = person.Id };
                    _db.EmailPreferences.Add(preference);
                    preferences[person.Id] = preference;
                }
                preference.LastDigestSentAt = _now;
                await _db.SaveChangesAsync(ct);
            }
        }

        private async Task<Dictionary<string, Person>> PeopleAsync(List<string>? ids, CancellationToken ct) =>
            await _db.Users.AsNoTracking()
                .Where(u => ids == null || ids.Contains(u.Id))
                .Select(u => new Person(u.Id, u.Email!, (u.FirstName + " " + u.LastName).Trim(), u.IsActive))
                .ToDictionaryAsync(p => p.Id, ct);
    }
}

using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NSBM.ICU.Application.Common.Auditing;
using NSBM.ICU.Application.Common.Extensions;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Application.Common.Notifications;
using NSBM.ICU.Domain.Entities.Notifications;
using NSBM.ICU.Domain.Enums;

namespace NSBM.ICU.Application.Features.Announcements.Commands.SendAnnouncement;

/// <summary>The Unit admin notifies everyone, or one group, in a single step. The sender is not notified.</summary>
public sealed record SendAnnouncementCommand(AnnouncementAudience Audience, string Title, string Message) : IRequest<int>;

public sealed class SendAnnouncementCommandValidator : AbstractValidator<SendAnnouncementCommand>
{
    public const int MaxMessageLength = 500;

    public SendAnnouncementCommandValidator()
    {
        RuleFor(x => x.Audience).IsInEnum();
        RuleFor(x => x.Title).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Message).NotEmpty().MaximumLength(MaxMessageLength);
    }
}

public sealed class SendAnnouncementCommandHandler(IApplicationDbContext db, ICurrentUser currentUser, IDateTime clock, IAuditTrail audit)
    : IRequestHandler<SendAnnouncementCommand, int>
{
    public async Task<int> Handle(SendAnnouncementCommand request, CancellationToken ct)
    {
        var admin = await db.GetCurrentAdministratorAsync(currentUser, ct);
        var recipients = (await RecipientsAsync(request.Audience, ct)).Where(id => id != currentUser.UserId).Distinct().ToList();

        var announcement = new Announcement
        {
            Audience = request.Audience,
            Title = request.Title.Trim(),
            Message = request.Message.Trim(),
            SentByName = admin.FullName,
            SentAt = clock.UtcNow,
            RecipientCount = recipients.Count,
        };
        db.Announcements.Add(announcement);
        db.Notify(recipients, NotificationKind.Announcement, announcement.Title, announcement.Message, NotificationLinks.Notifications, clock.UtcNow);

        await audit.RecordAsync(AuditCategory.Communication, AuditActions.AnnouncementSent,
            $"Sent \"{announcement.Title}\" to {request.Audience} ({recipients.Count} people)", cancellationToken: ct);
        await db.SaveChangesAsync(ct);
        return announcement.Id;
    }

    private async Task<IEnumerable<string>> RecipientsAsync(AnnouncementAudience audience, CancellationToken ct)
    {
        var include = (AnnouncementAudience group) => audience == AnnouncementAudience.Everyone || audience == group;
        var staff = audience is AnnouncementAudience.Everyone or AnnouncementAudience.Staff;

        var ids = new List<string>();
        if (include(AnnouncementAudience.Students)) ids.AddRange(await db.Students.Select(s => s.UserId).ToListAsync(ct));
        if (include(AnnouncementAudience.Partners)) ids.AddRange(await db.IndustryPartners.Select(p => p.UserId).ToListAsync(ct));
        if (staff || audience == AnnouncementAudience.Lecturers) ids.AddRange(await db.Lecturers.Select(l => l.UserId).ToListAsync(ct));
        if (staff)
        {
            ids.AddRange(await db.RegistrarUserIdsAsync(ct));
            ids.AddRange(await db.AdminUserIdsAsync(ct));
        }
        return ids;
    }
}

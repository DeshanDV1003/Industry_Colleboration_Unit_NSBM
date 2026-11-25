using System.Threading.Channels;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSBM.ICU.Application.Common.Email;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Application.Common.Models;
using NSBM.ICU.Infrastructure.Identity;

namespace NSBM.ICU.Infrastructure.Email;

/// <summary>
/// Queues reset links and sends them in the background. The token travels only in memory and in the e-mail;
/// it is never written to the database or logged.
/// </summary>
public sealed class PasswordResetMailer(IEmailSender sender, IOptions<EmailOptions> email, IOptions<PasswordResetOptions> reset,
    ILogger<PasswordResetMailer> logger) : BackgroundService, IPasswordResetMailer
{
    private readonly Channel<PasswordResetTicket> _queue = Channel.CreateBounded<PasswordResetTicket>(
        new BoundedChannelOptions(1000) { FullMode = BoundedChannelFullMode.DropOldest });

    public bool IsAvailable => sender.IsEnabled && !string.IsNullOrWhiteSpace(email.Value.AppBaseUrl);
    public int LinkValidMinutes => reset.Value.LinkValidMinutes;

    public void Enqueue(PasswordResetTicket ticket) => _queue.Writer.TryWrite(ticket);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var ticket in _queue.Reader.ReadAllAsync(stoppingToken))
        {
            var url = $"{email.Value.AppBaseUrl.TrimEnd('/')}/reset-password?email={Uri.EscapeDataString(ticket.Email)}&token={ticket.Token}";
            try
            {
                await sender.SendAsync(EmailTemplates.PasswordReset(ticket.Email, ticket.FullName, url, LinkValidMinutes), stoppingToken);
            }
            catch (Exception e) when (e is not OperationCanceledException)
            {
                // The person can simply ask again; the token is not kept for a retry.
                logger.LogWarning(e, "The password-reset e-mail for user {UserId} could not be sent.", ticket.UserId);
            }
        }
    }
}

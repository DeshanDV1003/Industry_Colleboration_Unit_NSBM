using NSBM.ICU.Application.Common.Email;
using NSBM.ICU.Application.Common.Interfaces;

namespace NSBM.ICU.Infrastructure.Email;

/// <summary>Used when e-mail is off: nothing is sent and notifications stay in the app.</summary>
public sealed class DisabledEmailSender : IEmailSender
{
    public bool IsEnabled => false;
    public Task SendAsync(EmailMessage message, CancellationToken cancellationToken) => Task.CompletedTask;
}

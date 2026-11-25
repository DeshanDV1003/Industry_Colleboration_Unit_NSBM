using MailKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using NSBM.ICU.Application.Common.Email;
using NSBM.ICU.Application.Common.Interfaces;

namespace NSBM.ICU.Infrastructure.Email;

/// <summary>Production: sends through the configured SMTP server (the university's mail relay or a mail service).</summary>
public sealed class SmtpEmailSender(IOptions<EmailOptions> options) : IEmailSender
{
    public bool IsEnabled => true;

    public async Task SendAsync(EmailMessage message, CancellationToken cancellationToken)
    {
        var smtp = options.Value.Smtp;
        var security = Enum.TryParse<SecureSocketOptions>(smtp.Security, ignoreCase: true, out var parsed) ? parsed : SecureSocketOptions.StartTls;

        using var client = new SmtpClient();
        await client.ConnectAsync(smtp.Host, smtp.Port, security, cancellationToken);
        if (!string.IsNullOrEmpty(smtp.Username))
            await client.AuthenticateAsync(smtp.Username, smtp.Password, cancellationToken);
        await client.SendAsync(new SendRequest(EmailMime.Build(message, options.Value)), cancellationToken);
        await client.DisconnectAsync(quit: true, cancellationToken);
    }
}

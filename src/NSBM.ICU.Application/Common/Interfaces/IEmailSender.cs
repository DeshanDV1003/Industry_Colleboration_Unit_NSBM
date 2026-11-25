using NSBM.ICU.Application.Common.Email;

namespace NSBM.ICU.Application.Common.Interfaces;

/// <summary>Delivers an e-mail: by SMTP in production, to a local folder in development, or nowhere when e-mail is off.</summary>
public interface IEmailSender
{
    bool IsEnabled { get; }

    Task SendAsync(EmailMessage message, CancellationToken cancellationToken);
}

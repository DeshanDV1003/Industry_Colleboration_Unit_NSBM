using MediatR;
using NSBM.ICU.Application.Common.Email;
using NSBM.ICU.Application.Common.Exceptions;
using NSBM.ICU.Application.Common.Extensions;
using NSBM.ICU.Application.Common.Interfaces;

namespace NSBM.ICU.Application.Features.Settings.Commands.SendTestEmail;

/// <summary>The Unit admin checks the e-mail settings by sending a message to their own address. Returns that address.</summary>
public sealed record SendTestEmailCommand : IRequest<string>;

public sealed class SendTestEmailCommandHandler(IApplicationDbContext db, ICurrentUser currentUser, IEmailSender email, IEmailLinks links)
    : IRequestHandler<SendTestEmailCommand, string>
{
    public async Task<string> Handle(SendTestEmailCommand request, CancellationToken ct)
    {
        var admin = await db.GetCurrentAdministratorAsync(currentUser, ct);
        if (!email.IsEnabled)
            throw new BusinessRuleException("E-mail is switched off on this server. Set Email__Mode to Smtp (or File for testing).");

        try
        {
            await email.SendAsync(EmailTemplates.Test(admin.Email, admin.FullName, links.AppBaseUrl), ct);
        }
        catch (Exception e) when (e is not OperationCanceledException)
        {
            throw new BusinessRuleException($"The e-mail could not be sent: {e.Message}");
        }
        return admin.Email;
    }
}

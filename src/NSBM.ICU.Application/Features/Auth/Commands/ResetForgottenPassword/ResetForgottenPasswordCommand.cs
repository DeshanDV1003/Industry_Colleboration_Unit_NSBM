using FluentValidation;
using MediatR;
using NSBM.ICU.Application.Common.Auditing;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Application.Common.Notifications;
using NSBM.ICU.Application.Common.Validation;
using NSBM.ICU.Domain.Enums;

namespace NSBM.ICU.Application.Features.Auth.Commands.ResetForgottenPassword;

/// <summary>Sets a new password from an e-mailed link. The link then stops working, as does every older one.</summary>
public sealed record ResetForgottenPasswordCommand(string Email, string Token, string NewPassword) : IRequest;

public sealed class ResetForgottenPasswordCommandValidator : AbstractValidator<ResetForgottenPasswordCommand>
{
    public ResetForgottenPasswordCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Token).NotEmpty().WithMessage("This link is incomplete. Open the link from the e-mail again.").MaximumLength(2000);
        RuleFor(x => x.NewPassword).ValidPassword();
    }
}

public sealed class ResetForgottenPasswordCommandHandler(IApplicationDbContext db, IIdentityService identity, IAuditTrail audit)
    : IRequestHandler<ResetForgottenPasswordCommand>
{
    public async Task Handle(ResetForgottenPasswordCommand request, CancellationToken ct)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var userId = await identity.ResetPasswordWithTokenAsync(email, request.Token, request.NewPassword, ct);

        db.NotifySecurity(userId, "Your password was reset",
            "Your password was changed using a reset link sent to your e-mail. If this was not you, contact the Industry Collaboration Unit straight away.");
        await audit.RecordForAsync(userId, AuditCategory.Accounts, AuditActions.PasswordResetByLink,
            "Reset their own password with an e-mailed link", "User", email, ct);
        await db.SaveChangesAsync(ct);
    }
}

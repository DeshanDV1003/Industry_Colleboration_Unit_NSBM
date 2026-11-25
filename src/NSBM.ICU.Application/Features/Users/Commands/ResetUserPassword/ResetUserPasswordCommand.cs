using FluentValidation;
using MediatR;
using NSBM.ICU.Application.Common.Auditing;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Application.Common.Validation;
using NSBM.ICU.Domain.Enums;

namespace NSBM.ICU.Application.Features.Users.Commands.ResetUserPassword;

/// <summary>The Unit admin sets a temporary password for someone who forgot theirs.</summary>
public sealed record ResetUserPasswordCommand(string UserId, string NewPassword) : IRequest;

public sealed class ResetUserPasswordCommandValidator : AbstractValidator<ResetUserPasswordCommand>
{
    public ResetUserPasswordCommandValidator() => RuleFor(x => x.NewPassword).ValidPassword();
}

public sealed class ResetUserPasswordCommandHandler(IApplicationDbContext db, IIdentityService identity, IAuditTrail audit)
    : IRequestHandler<ResetUserPasswordCommand>
{
    public async Task Handle(ResetUserPasswordCommand request, CancellationToken ct)
    {
        var profile = await db.RequireAsync(request.UserId, ct);
        await audit.RecordAsync(AuditCategory.Accounts, AuditActions.PasswordReset, $"Reset the password of {profile.FullName}", "User", profile.Email, ct);
        await identity.ResetPasswordAsync(request.UserId, request.NewPassword, ct);
        await db.SaveChangesAsync(ct);
    }
}

using FluentValidation;
using MediatR;
using NSBM.ICU.Application.Common.Auditing;
using NSBM.ICU.Application.Common.Exceptions;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Application.Common.Validation;
using NSBM.ICU.Application.Features.Users;
using NSBM.ICU.Domain.Enums;

namespace NSBM.ICU.Application.Features.Auth.Commands.ChangePassword;

/// <summary>Anyone signed in changes their own password, e.g. replacing the temporary one the admin gave them.</summary>
public sealed record ChangePasswordCommand(string CurrentPassword, string NewPassword) : IRequest;

public sealed class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordCommandValidator()
    {
        RuleFor(x => x.CurrentPassword).NotEmpty().WithMessage("Enter your current password.");
        RuleFor(x => x.NewPassword).ValidPassword()
            .NotEqual(x => x.CurrentPassword).WithMessage("Choose a password different from the current one.");
    }
}

public sealed class ChangePasswordCommandHandler(IApplicationDbContext db, IIdentityService identity, ICurrentUser currentUser, IAuditTrail audit)
    : IRequestHandler<ChangePasswordCommand>
{
    public async Task Handle(ChangePasswordCommand request, CancellationToken ct)
    {
        var userId = currentUser.UserId ?? throw new AuthenticationFailedException("You are not signed in.");
        var email = (await db.RequireAsync(userId, ct)).Email;
        await audit.RecordAsync(AuditCategory.Accounts, AuditActions.PasswordChanged, "Changed their own password", "User", email, ct);
        await identity.ChangePasswordAsync(userId, request.CurrentPassword, request.NewPassword, ct);
        await db.SaveChangesAsync(ct);
    }
}

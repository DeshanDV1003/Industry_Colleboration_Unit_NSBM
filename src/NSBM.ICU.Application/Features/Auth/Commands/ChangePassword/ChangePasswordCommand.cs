using FluentValidation;
using MediatR;
using NSBM.ICU.Application.Common.Exceptions;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Application.Common.Validation;

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

public sealed class ChangePasswordCommandHandler(IIdentityService identity, ICurrentUser currentUser) : IRequestHandler<ChangePasswordCommand>
{
    public Task Handle(ChangePasswordCommand request, CancellationToken ct) =>
        identity.ChangePasswordAsync(currentUser.UserId ?? throw new AuthenticationFailedException("You are not signed in."),
            request.CurrentPassword, request.NewPassword, ct);
}

using FluentValidation;
using MediatR;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Application.Common.Validation;

namespace NSBM.ICU.Application.Features.Users.Commands.ResetUserPassword;

/// <summary>The Unit admin sets a temporary password for someone who forgot theirs.</summary>
public sealed record ResetUserPasswordCommand(string UserId, string NewPassword) : IRequest;

public sealed class ResetUserPasswordCommandValidator : AbstractValidator<ResetUserPasswordCommand>
{
    public ResetUserPasswordCommandValidator() => RuleFor(x => x.NewPassword).ValidPassword();
}

public sealed class ResetUserPasswordCommandHandler(IIdentityService identity) : IRequestHandler<ResetUserPasswordCommand>
{
    public Task Handle(ResetUserPasswordCommand request, CancellationToken ct) =>
        identity.ResetPasswordAsync(request.UserId, request.NewPassword, ct);
}

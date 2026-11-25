using FluentValidation;
using MediatR;
using NSBM.ICU.Application.Common.Exceptions;
using NSBM.ICU.Application.Common.Interfaces;

namespace NSBM.ICU.Application.Features.Auth.Commands.ForgotPassword;

/// <summary>
/// Someone signed out asks for a reset link. The answer never says whether the address has an account; when it
/// belongs to an active account, a single-use link is e-mailed to that address only.
/// </summary>
public sealed record ForgotPasswordCommand(string Email) : IRequest;

public sealed class ForgotPasswordCommandValidator : AbstractValidator<ForgotPasswordCommand>
{
    public ForgotPasswordCommandValidator() => RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(256);
}

public sealed class ForgotPasswordCommandHandler(IIdentityService identity, IPasswordResetMailer mailer) : IRequestHandler<ForgotPasswordCommand>
{
    public async Task Handle(ForgotPasswordCommand request, CancellationToken ct)
    {
        if (!mailer.IsAvailable)
            throw new BusinessRuleException("Password reset by e-mail is not available on this server. Ask the Industry Collaboration Unit to reset your password.");

        var ticket = await identity.CreatePasswordResetAsync(request.Email.Trim(), ct);
        if (ticket is not null)
            mailer.Enqueue(ticket);
    }
}

using FluentValidation;
using MediatR;
using NSBM.ICU.Application.Common.Exceptions;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Application.Features.Auth.Dtos;
using NSBM.ICU.Domain.Constants;

namespace NSBM.ICU.Application.Features.Auth.Commands.DemoLogin;

/// <summary>Signs in as the demo account of a role. Powers the demo role switcher; disabled outside demo mode.</summary>
public sealed record DemoLoginCommand(string Role) : IRequest<AuthResponseDto>;

public sealed class DemoLoginCommandValidator : AbstractValidator<DemoLoginCommand>
{
    public DemoLoginCommandValidator()
    {
        RuleFor(x => x.Role)
            .Must(role => Roles.All.Contains(role))
            .WithMessage($"Role must be one of: {string.Join(", ", Roles.All)}.");
    }
}

public sealed class DemoLoginCommandHandler(IIdentityService identity, ITokenService tokens)
    : IRequestHandler<DemoLoginCommand, AuthResponseDto>
{
    public async Task<AuthResponseDto> Handle(DemoLoginCommand request, CancellationToken cancellationToken)
    {
        var user = await identity.FindDemoAccountAsync(request.Role, cancellationToken)
                   ?? throw new ForbiddenAccessException("Demo sign-in is not available.");

        return AuthResponseDto.From(user, tokens.CreateToken(user));
    }
}

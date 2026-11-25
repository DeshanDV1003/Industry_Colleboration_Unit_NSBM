using FluentValidation;
using MediatR;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Application.Common.Models;
using NSBM.ICU.Application.Common.Validation;
using NSBM.ICU.Application.Features.Users.Models;
using NSBM.ICU.Domain.Constants;

namespace NSBM.ICU.Application.Features.Users.Commands.CreateUser;

/// <summary>The Unit admin opens an account for any role with a temporary password to hand over.</summary>
public sealed record CreateUserCommand(string Role, string Email, string Password, UserProfileInput Profile) : IRequest<string>;

public sealed class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.Role).Must(r => Roles.All.Contains(r)).WithMessage("Choose a role.");
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(256);
        RuleFor(x => x.Password).ValidPassword();
        RuleFor(x => x.Profile).NotNull()
            .SetValidator(x => new UserProfileInputValidator(x.Role)).When(x => Roles.All.Contains(x.Role));
    }
}

public sealed class CreateUserCommandHandler(IApplicationDbContext db, IIdentityService identity)
    : IRequestHandler<CreateUserCommand, string>
{
    public async Task<string> Handle(CreateUserCommand request, CancellationToken ct)
    {
        var profile = UserProfiles.New(request.Role);
        UserProfiles.Apply(profile, request.Profile);
        await db.EnsureValidAsync(profile, ct);

        var account = new NewAccount(request.Role, request.Email.Trim().ToLowerInvariant(), profile.FirstName, profile.LastName, request.Password);
        return await identity.CreateAccountAsync(account, profile, ct);
    }
}

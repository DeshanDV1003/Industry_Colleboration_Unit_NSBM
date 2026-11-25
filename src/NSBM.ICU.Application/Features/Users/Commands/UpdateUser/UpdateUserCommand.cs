using FluentValidation;
using MediatR;
using NSBM.ICU.Application.Common.Auditing;
using NSBM.ICU.Application.Common.Exceptions;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Application.Features.Users.Models;
using NSBM.ICU.Domain.Constants;
using NSBM.ICU.Domain.Enums;

namespace NSBM.ICU.Application.Features.Users.Commands.UpdateUser;

/// <summary>
/// Edits an account's name and role details. The role is sent so the right fields are validated; it must match
/// the account's role, which never changes. E-mail is the sign-in name and is not edited here.
/// </summary>
public sealed record UpdateUserCommand(string UserId, string Role, UserProfileInput Profile) : IRequest;

public sealed class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(x => x.Role).Must(r => Roles.All.Contains(r)).WithMessage("Choose a role.");
        RuleFor(x => x.Profile).NotNull()
            .SetValidator(x => new UserProfileInputValidator(x.Role)).When(x => Roles.All.Contains(x.Role));
    }
}

public sealed class UpdateUserCommandHandler(IApplicationDbContext db, IIdentityService identity, IAuditTrail audit) : IRequestHandler<UpdateUserCommand>
{
    public async Task Handle(UpdateUserCommand request, CancellationToken ct)
    {
        var statuses = await identity.GetAccountStatusesAsync(ct);
        if (!statuses.TryGetValue(request.UserId, out var account))
            throw NotFoundException.For("User", request.UserId);
        if (account.Role != request.Role)
            throw new BusinessRuleException("An account's role cannot be changed.");

        var profile = await db.RequireAsync(request.UserId, ct);
        UserProfiles.Apply(profile, request.Profile);
        await db.EnsureValidAsync(profile, ct);

        await audit.RecordAsync(AuditCategory.Accounts, AuditActions.UserUpdated,
            $"Updated the account of {profile.FullName} ({account.Role})", "User", account.Email, ct);
        await db.SaveChangesAsync(ct);
        await identity.RenameAsync(request.UserId, profile.FirstName, profile.LastName, ct);
    }
}

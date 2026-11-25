using MediatR;
using NSBM.ICU.Application.Common.Exceptions;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Domain.Constants;

namespace NSBM.ICU.Application.Features.Users.Commands.SetUserActive;

/// <summary>
/// Deactivates an account (it can no longer sign in and its open sessions stop working) or reactivates it.
/// History is kept. An admin cannot lock themself out, and the Unit always keeps one active admin.
/// </summary>
public sealed record SetUserActiveCommand(string UserId, bool IsActive) : IRequest;

public sealed class SetUserActiveCommandHandler(IIdentityService identity, ICurrentUser currentUser) : IRequestHandler<SetUserActiveCommand>
{
    public async Task Handle(SetUserActiveCommand request, CancellationToken ct)
    {
        var statuses = await identity.GetAccountStatusesAsync(ct);
        if (!statuses.TryGetValue(request.UserId, out var account))
            throw NotFoundException.For("User", request.UserId);

        if (!request.IsActive)
        {
            if (request.UserId == currentUser.UserId)
                throw new BusinessRuleException("You cannot deactivate your own account.");
            if (account.Role == Roles.Admin && statuses.Values.Count(a => a.Role == Roles.Admin && a.IsActive) <= 1)
                throw new BusinessRuleException("The Unit must keep at least one active admin.");
        }

        await identity.SetActiveAsync(request.UserId, request.IsActive, ct);
    }
}

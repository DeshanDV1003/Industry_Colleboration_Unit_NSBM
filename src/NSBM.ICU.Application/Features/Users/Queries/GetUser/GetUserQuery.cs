using MediatR;
using NSBM.ICU.Application.Common.Exceptions;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Application.Features.Users.Dtos;

namespace NSBM.ICU.Application.Features.Users.Queries.GetUser;

/// <summary>One account with its editable details.</summary>
public sealed record GetUserQuery(string UserId) : IRequest<UserDetailDto>;

public sealed class GetUserQueryHandler(IApplicationDbContext db, IIdentityService identity) : IRequestHandler<GetUserQuery, UserDetailDto>
{
    public async Task<UserDetailDto> Handle(GetUserQuery request, CancellationToken ct)
    {
        var statuses = await identity.GetAccountStatusesAsync(ct);
        if (!statuses.TryGetValue(request.UserId, out var account))
            throw NotFoundException.For("User", request.UserId);

        var profile = await db.RequireAsync(request.UserId, ct);
        return new UserDetailDto(account.UserId, account.Role, account.Email, account.IsActive, UserProfiles.ToInput(profile));
    }
}

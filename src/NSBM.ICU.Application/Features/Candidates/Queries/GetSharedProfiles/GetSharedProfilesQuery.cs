using MediatR;
using Microsoft.EntityFrameworkCore;
using NSBM.ICU.Application.Common.Extensions;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Application.Features.Candidates.Dtos;

namespace NSBM.ICU.Application.Features.Candidates.Queries.GetSharedProfiles;

/// <summary>Students who opted in to sharing their profile with partner companies.</summary>
public sealed record GetSharedProfilesQuery : IRequest<IReadOnlyList<SharedProfileDto>>;

public sealed class GetSharedProfilesQueryHandler(IApplicationDbContext db, ICurrentUser currentUser)
    : IRequestHandler<GetSharedProfilesQuery, IReadOnlyList<SharedProfileDto>>
{
    public async Task<IReadOnlyList<SharedProfileDto>> Handle(GetSharedProfilesQuery request, CancellationToken cancellationToken)
    {
        await db.GetCurrentPartnerAsync(currentUser, cancellationToken);

        return await db.Students.AsNoTracking()
            .Where(s => s.ShareProfileWithPartners)
            .OrderBy(s => s.FirstName).ThenBy(s => s.LastName)
            .Select(s => new SharedProfileDto(s.Id, s.FirstName + " " + s.LastName, s.StudentNo, s.Programme,
                s.CareerPath != null ? s.CareerPath.Name : null))
            .ToListAsync(cancellationToken);
    }
}

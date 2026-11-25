using MediatR;
using Microsoft.EntityFrameworkCore;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Application.Features.Careers.Dtos;
using NSBM.ICU.Domain.Enums;

namespace NSBM.ICU.Application.Features.Careers.Queries.GetCareerPaths;

public sealed record GetCareerPathsQuery : IRequest<IReadOnlyList<CareerPathSummaryDto>>;

public sealed class GetCareerPathsQueryHandler(IApplicationDbContext db, ICurrentUser currentUser)
    : IRequestHandler<GetCareerPathsQuery, IReadOnlyList<CareerPathSummaryDto>>
{
    public async Task<IReadOnlyList<CareerPathSummaryDto>> Handle(GetCareerPathsQuery request, CancellationToken cancellationToken)
    {
        var selectedId = await db.Students.AsNoTracking()
            .Where(s => s.UserId == currentUser.UserId)
            .Select(s => s.CareerPathId)
            .FirstOrDefaultAsync(cancellationToken);

        var paths = await db.CareerPaths.AsNoTracking()
            .OrderBy(p => p.SortOrder)
            .Select(p => new
            {
                p.Id,
                p.Slug,
                p.Name,
                p.Tagline,
                p.EntryRoles,
                CoreSkills = p.Skills
                    .Where(s => s.Importance == SkillImportance.Core)
                    .OrderBy(s => s.Skill!.Name)
                    .Select(s => s.Skill!.Name)
                    .ToList(),
                SkillCount = p.Skills.Count,
                RoadmapStepCount = p.RoadmapSteps.Count,
            })
            .ToListAsync(cancellationToken);

        return paths
            .Select(p => new CareerPathSummaryDto(p.Id, p.Slug, p.Name, p.Tagline,
                CareerPathFormatting.SplitEntryRoles(p.EntryRoles), p.CoreSkills, p.SkillCount, p.RoadmapStepCount,
                IsSelected: p.Id == selectedId))
            .ToList();
    }
}

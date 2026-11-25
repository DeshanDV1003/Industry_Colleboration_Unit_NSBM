using MediatR;
using Microsoft.EntityFrameworkCore;
using NSBM.ICU.Application.Common.Extensions;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Application.Features.Roadmap.Dtos;
using NSBM.ICU.Domain.Rules;

namespace NSBM.ICU.Application.Features.Roadmap.Queries.GetMyRoadmap;

/// <summary>Steps from beginner to advanced for the student's chosen path, with how far they have come.</summary>
public sealed record GetMyRoadmapQuery : IRequest<RoadmapDto>;

public sealed class GetMyRoadmapQueryHandler(IApplicationDbContext db, ICurrentUser currentUser)
    : IRequestHandler<GetMyRoadmapQuery, RoadmapDto>
{
    public async Task<RoadmapDto> Handle(GetMyRoadmapQuery request, CancellationToken cancellationToken)
    {
        var student = await db.GetCurrentStudentAsync(currentUser, cancellationToken);
        if (student.CareerPathId is not { } pathId)
            return new RoadmapDto(null, null, 0, 0, 0, []);

        var pathName = await db.CareerPaths.Where(p => p.Id == pathId).Select(p => p.Name).SingleAsync(cancellationToken);

        var completions = await db.RoadmapStepCompletions.AsNoTracking()
            .Where(c => c.StudentId == student.Id && c.RoadmapStep!.CareerPathId == pathId)
            .ToDictionaryAsync(c => c.RoadmapStepId, c => c.CompletedAt, cancellationToken);

        var steps = await db.RoadmapSteps.AsNoTracking()
            .Where(s => s.CareerPathId == pathId)
            .OrderBy(s => s.SortOrder)
            .Select(s => new { s.Id, s.SortOrder, s.Stage, s.Title, s.Description, s.Outcome, SkillName = s.Skill != null ? s.Skill.Name : null })
            .ToListAsync(cancellationToken);

        var stepDtos = steps
            .Select(s => new RoadmapStepDto(s.Id, s.SortOrder, s.Stage, s.Title, s.Description, s.Outcome, s.SkillName,
                completions.ContainsKey(s.Id), completions.TryGetValue(s.Id, out var at) ? at : null))
            .ToList();

        var completed = stepDtos.Count(s => s.IsCompleted);
        return new RoadmapDto(pathId, pathName, completed, stepDtos.Count, SkillSignalRules.Percent(completed, stepDtos.Count), stepDtos);
    }
}

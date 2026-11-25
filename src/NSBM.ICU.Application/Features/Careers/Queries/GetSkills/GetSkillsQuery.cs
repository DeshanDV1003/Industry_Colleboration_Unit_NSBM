using MediatR;
using Microsoft.EntityFrameworkCore;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Application.Features.Careers.Dtos;

namespace NSBM.ICU.Application.Features.Careers.Queries.GetSkills;

/// <summary>The full skills catalogue, used when tagging tasks and vacancies.</summary>
public sealed record GetSkillsQuery : IRequest<IReadOnlyList<SkillDto>>;

public sealed class GetSkillsQueryHandler(IApplicationDbContext db) : IRequestHandler<GetSkillsQuery, IReadOnlyList<SkillDto>>
{
    public async Task<IReadOnlyList<SkillDto>> Handle(GetSkillsQuery request, CancellationToken cancellationToken) =>
        await db.Skills.AsNoTracking()
            .OrderBy(s => s.Category).ThenBy(s => s.Name)
            .Select(s => new SkillDto(s.Id, s.Name, s.Category))
            .ToListAsync(cancellationToken);
}

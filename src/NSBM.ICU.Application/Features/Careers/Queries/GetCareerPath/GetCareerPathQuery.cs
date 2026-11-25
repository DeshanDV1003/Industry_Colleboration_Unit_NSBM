using MediatR;
using Microsoft.EntityFrameworkCore;
using NSBM.ICU.Application.Common.Exceptions;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Application.Features.Careers.Dtos;

namespace NSBM.ICU.Application.Features.Careers.Queries.GetCareerPath;

public sealed record GetCareerPathQuery(string Slug) : IRequest<CareerPathDetailDto>;

public sealed class GetCareerPathQueryHandler(IApplicationDbContext db, ICurrentUser currentUser)
    : IRequestHandler<GetCareerPathQuery, CareerPathDetailDto>
{
    public async Task<CareerPathDetailDto> Handle(GetCareerPathQuery request, CancellationToken cancellationToken)
    {
        var path = await db.CareerPaths.AsNoTracking()
            .Where(p => p.Slug == request.Slug)
            .Select(p => new
            {
                p.Id,
                p.Slug,
                p.Name,
                p.Tagline,
                p.Description,
                p.EntryRoles,
                Skills = p.Skills
                    .OrderBy(s => s.Importance).ThenBy(s => s.Skill!.Name)
                    .Select(s => new CareerSkillDto(s.SkillId, s.Skill!.Name, s.Skill.Category, s.Importance, s.Skill.Description))
                    .ToList(),
            })
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw NotFoundException.For("Career path", request.Slug);

        var isSelected = await db.Students.AnyAsync(
            s => s.UserId == currentUser.UserId && s.CareerPathId == path.Id, cancellationToken);

        return new CareerPathDetailDto(path.Id, path.Slug, path.Name, path.Tagline, path.Description,
            CareerPathFormatting.SplitEntryRoles(path.EntryRoles), path.Skills, isSelected);
    }
}

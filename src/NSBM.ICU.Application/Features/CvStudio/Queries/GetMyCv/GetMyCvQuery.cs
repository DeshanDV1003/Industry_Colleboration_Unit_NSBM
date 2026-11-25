using MediatR;
using Microsoft.EntityFrameworkCore;
using NSBM.ICU.Application.Common.Extensions;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Application.Features.CvStudio.Dtos;

namespace NSBM.ICU.Application.Features.CvStudio.Queries.GetMyCv;

public sealed record GetMyCvQuery : IRequest<CvDto>;

public sealed class GetMyCvQueryHandler(IApplicationDbContext db, ICurrentUser currentUser) : IRequestHandler<GetMyCvQuery, CvDto>
{
    public async Task<CvDto> Handle(GetMyCvQuery request, CancellationToken cancellationToken)
    {
        var student = await db.GetCurrentStudentAsync(currentUser, cancellationToken);
        var cv = await db.CvProfiles.AsNoTracking().FirstOrDefaultAsync(c => c.StudentId == student.Id, cancellationToken);
        var pathName = student.CareerPathId is { } pathId
            ? await db.CareerPaths.Where(p => p.Id == pathId).Select(p => p.Name).FirstOrDefaultAsync(cancellationToken)
            : null;
        var skills = await db.StudentSkills.Where(s => s.StudentId == student.Id).OrderBy(s => s.Skill!.Name)
            .Select(s => s.Skill!.Name).ToListAsync(cancellationToken);

        return new CvDto(student.FullName, student.Email, student.StudentNo, student.Programme, pathName,
            cv?.Headline ?? string.Empty, cv?.Summary ?? string.Empty, cv?.Education ?? string.Empty, cv?.Projects ?? string.Empty,
            cv?.GitHubUrl, cv?.LinkedInUrl, skills);
    }
}

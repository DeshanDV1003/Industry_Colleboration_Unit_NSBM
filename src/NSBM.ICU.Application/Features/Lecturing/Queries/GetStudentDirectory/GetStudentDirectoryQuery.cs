using MediatR;
using Microsoft.EntityFrameworkCore;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Application.Features.Lecturing.Dtos;
using NSBM.ICU.Domain.Rules;

namespace NSBM.ICU.Application.Features.Lecturing.Queries.GetStudentDirectory;

/// <summary>Students with role, task and progress facts. Sorted by name — the directory never ranks students.</summary>
public sealed record GetStudentDirectoryQuery(string? Search, bool NeedsFollowUpOnly) : IRequest<IReadOnlyList<StudentDirectoryItemDto>>;

public sealed class GetStudentDirectoryQueryHandler(IApplicationDbContext db, IDateTime clock)
    : IRequestHandler<GetStudentDirectoryQuery, IReadOnlyList<StudentDirectoryItemDto>>
{
    public async Task<IReadOnlyList<StudentDirectoryItemDto>> Handle(GetStudentDirectoryQuery request, CancellationToken cancellationToken)
    {
        var students = db.Students.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var term = request.Search.Trim();
            students = students.Where(s => (s.FirstName + " " + s.LastName).Contains(term) || s.StudentNo.Contains(term)
                                           || (s.CareerPath != null && s.CareerPath.Name.Contains(term)));
        }

        var rows = await students.OrderBy(s => s.FirstName).ThenBy(s => s.LastName)
            .Select(s => new { s.Id, Name = s.FirstName + " " + s.LastName, s.StudentNo, s.Programme, Path = s.CareerPath != null ? s.CareerPath.Name : null })
            .ToListAsync(cancellationToken);

        var progress = await db.LoadProgressAsync(rows.Select(r => r.Id).ToList(), clock.UtcNow, cancellationToken);

        return rows
            .Select(r =>
            {
                var p = progress[r.Id];
                return new StudentDirectoryItemDto(r.Id, r.Name, r.StudentNo, r.Programme, r.Path, p.TasksAttempted,
                    SkillSignalRules.Percent(p.RoadmapCompleted, p.RoadmapTotal), p.LastActivityAt, p.Signals);
            })
            .Where(r => !request.NeedsFollowUpOnly || r.Signals.Count > 0)
            .ToList();
    }
}

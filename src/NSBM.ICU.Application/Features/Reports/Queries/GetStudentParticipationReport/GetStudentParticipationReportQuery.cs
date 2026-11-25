using MediatR;
using Microsoft.EntityFrameworkCore;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Application.Common.Reports;
using NSBM.ICU.Domain.Enums;

namespace NSBM.ICU.Application.Features.Reports.Queries.GetStudentParticipationReport;

/// <summary>
/// What each student has taken part in, as counts, for planning support. Students are listed by number,
/// never ranked or scored: the platform's responsible-use rules forbid automatic ranking.
/// </summary>
public sealed record GetStudentParticipationReportQuery(string? Programme, int? AcademicYear) : IRequest<ReportTable>;

public sealed class GetStudentParticipationReportQueryHandler(IApplicationDbContext db) : IRequestHandler<GetStudentParticipationReportQuery, ReportTable>
{
    private static readonly ReportColumn[] Columns =
    [
        new("Student no."), new("Student"), new("Programme"), new("Year", ReportColumnKind.Number), new("Career path"),
        new("Baseline taken"), new("Roadmap steps done", ReportColumnKind.Number), new("Roadmap steps", ReportColumnKind.Number),
        new("Tasks tried", ReportColumnKind.Number), new("Events attended", ReportColumnKind.Number), new("Badges", ReportColumnKind.Number),
        new("Applications", ReportColumnKind.Number),
    ];

    public async Task<ReportTable> Handle(GetStudentParticipationReportQuery request, CancellationToken ct)
    {
        var students = await db.Students.AsNoTracking()
            .Where(s => string.IsNullOrEmpty(request.Programme) || s.Programme == request.Programme)
            .Where(s => request.AcademicYear == null || s.AcademicYear == request.AcademicYear)
            .OrderBy(s => s.StudentNo)
            .Select(s => new
            {
                s.StudentNo, Name = s.FirstName + " " + s.LastName, s.Programme, s.AcademicYear, Path = s.CareerPath != null ? s.CareerPath.Name : null,
                Baseline = s.CareerPathId != null && db.BaselineAttempts.Any(b => b.StudentId == s.Id && b.CareerPathId == s.CareerPathId),
                StepsDone = db.RoadmapStepCompletions.Count(c => c.StudentId == s.Id && c.RoadmapStep!.CareerPathId == s.CareerPathId),
                Steps = db.RoadmapSteps.Count(r => r.CareerPathId == s.CareerPathId),
                Tasks = db.TaskAttempts.Where(t => t.StudentId == s.Id).Select(t => t.TaskId).Distinct().Count(),
                Events = db.EventRegistrations.Count(r => r.StudentId == s.Id && r.Attended),
                Badges = db.SkillBadges.Count(b => b.StudentId == s.Id && b.Status == ModerationStatus.Approved),
                Applications = db.InternshipApplications.Count(a => a.StudentId == s.Id),
            })
            .ToListAsync(ct);

        var rows = students.Select(s => new object?[]
        {
            s.StudentNo, s.Name, s.Programme, s.AcademicYear, s.Path ?? "Not chosen", s.Baseline, s.StepsDone, s.Steps, s.Tasks, s.Events, s.Badges, s.Applications,
        }).ToList();

        var scope = string.IsNullOrEmpty(request.Programme) ? "All students" : request.Programme + (request.AcademicYear is { } y ? $" · Year {y}" : string.Empty);
        return new ReportTable("Student participation", $"{scope} · {rows.Count} students · counts for planning support, not a ranking", Columns, rows)
        {
            FileStem = "student-participation",
        };
    }
}

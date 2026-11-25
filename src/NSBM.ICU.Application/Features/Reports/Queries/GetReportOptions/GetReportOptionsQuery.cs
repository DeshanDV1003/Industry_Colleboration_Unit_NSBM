using MediatR;
using Microsoft.EntityFrameworkCore;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Domain.Enums;

namespace NSBM.ICU.Application.Features.Reports.Queries.GetReportOptions;

public sealed record ReportOptionDto(int Id, string Name);
public sealed record ReportCohortDto(string Programme, int AcademicYear);

/// <summary>Filter choices for the report forms.</summary>
public sealed record ReportOptionsDto(IReadOnlyList<ReportOptionDto> Venues, IReadOnlyList<ReportCohortDto> Cohorts, IReadOnlyList<ReportOptionDto> Events);

public sealed record GetReportOptionsQuery : IRequest<ReportOptionsDto>;

public sealed class GetReportOptionsQueryHandler(IApplicationDbContext db) : IRequestHandler<GetReportOptionsQuery, ReportOptionsDto>
{
    public async Task<ReportOptionsDto> Handle(GetReportOptionsQuery request, CancellationToken ct)
    {
        var venues = await db.Venues.AsNoTracking().OrderBy(v => v.Code).Select(v => new ReportOptionDto(v.Id, v.Code + " · " + v.Name)).ToListAsync(ct);
        var cohorts = await db.Students.AsNoTracking()
            .Select(s => new { s.Programme, s.AcademicYear }).Distinct()
            .OrderBy(c => c.Programme).ThenBy(c => c.AcademicYear)
            .Select(c => new ReportCohortDto(c.Programme, c.AcademicYear))
            .ToListAsync(ct);
        var events = await db.IndustryEvents.AsNoTracking()
            .Where(e => e.Status == ModerationStatus.Approved)
            .OrderByDescending(e => e.StartDate)
            .Select(e => new ReportOptionDto(e.Id, e.Title))
            .ToListAsync(ct);
        return new ReportOptionsDto(venues, cohorts, events);
    }
}

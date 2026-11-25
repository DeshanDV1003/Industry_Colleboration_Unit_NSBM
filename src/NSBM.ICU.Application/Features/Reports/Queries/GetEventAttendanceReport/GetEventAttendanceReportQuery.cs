using MediatR;
using Microsoft.EntityFrameworkCore;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Application.Common.Reports;
using NSBM.ICU.Domain.Enums;

namespace NSBM.ICU.Application.Features.Reports.Queries.GetEventAttendanceReport;

/// <summary>Who registered for approved partner events, who attended and which certificates were issued; one event or all.</summary>
public sealed record GetEventAttendanceReportQuery(int? EventId) : IRequest<ReportTable>;

public sealed class GetEventAttendanceReportQueryHandler(IApplicationDbContext db) : IRequestHandler<GetEventAttendanceReportQuery, ReportTable>
{
    private static readonly ReportColumn[] Columns =
    [
        new("Event"), new("Host company"), new("Starts", ReportColumnKind.Date), new("Student no."), new("Student"), new("Programme"),
        new("Team"), new("Registered", ReportColumnKind.Date), new("Attended"), new("Certificate code"),
    ];

    public async Task<ReportTable> Handle(GetEventAttendanceReportQuery request, CancellationToken ct)
    {
        var registrations = await db.EventRegistrations.AsNoTracking()
            .Where(r => request.EventId == null || r.EventId == request.EventId)
            .Join(db.IndustryEvents.Where(e => e.Status == ModerationStatus.Approved), r => r.EventId, e => e.Id, (r, e) => new
            {
                Event = e.Title, Company = e.Company!.Name, e.StartDate, r.Student!.StudentNo, Student = r.Student.FirstName + " " + r.Student.LastName,
                r.Student.Programme, Team = r.Team != null ? r.Team.Name : null, r.RegisteredAt, r.Attended, r.CertificateCode,
            })
            .OrderBy(x => x.StartDate).ThenBy(x => x.Event).ThenBy(x => x.Student)
            .ToListAsync(ct);

        var rows = registrations.Select(r => new object?[]
        {
            r.Event, r.Company, r.StartDate, r.StudentNo, r.Student, r.Programme, r.Team, DateOnly.FromDateTime(r.RegisteredAt), r.Attended, r.CertificateCode,
        }).ToList();

        var scope = request.EventId is { } id
            ? await db.IndustryEvents.Where(e => e.Id == id).Select(e => e.Title).FirstOrDefaultAsync(ct) ?? "Unknown event"
            : "All approved partner events";
        var attended = registrations.Count(r => r.Attended);
        return new ReportTable("Event attendance", $"{scope} · {registrations.Count} registered, {attended} attended", Columns, rows) { FileStem = "event-attendance" };
    }
}

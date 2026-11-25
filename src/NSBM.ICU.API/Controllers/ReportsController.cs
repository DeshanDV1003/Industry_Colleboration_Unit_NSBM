using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Application.Common.Reports;
using NSBM.ICU.Application.Features.Reports.Queries.GetApplicationsReport;
using NSBM.ICU.Application.Features.Reports.Queries.GetEventAttendanceReport;
using NSBM.ICU.Application.Features.Reports.Queries.GetHallTimetableReport;
using NSBM.ICU.Application.Features.Reports.Queries.GetReportOptions;
using NSBM.ICU.Application.Features.Reports.Queries.GetStudentParticipationReport;
using NSBM.ICU.Domain.Constants;
using NSBM.ICU.Domain.Enums;

namespace NSBM.ICU.API.Controllers;

/// <summary>
/// Reports as a table (format=json, the default, for the on-screen preview and printing) or an Excel file
/// (format=xlsx). The Unit admin sees every report; the registrar the hall timetable; lecturers student participation.
/// </summary>
[Authorize]
[Route(ApiRoutes.V1 + "/reports")]
public sealed class ReportsController(IReportFileWriter writer, IDateTime clock) : ApiControllerBase
{
    private const string Excel = "xlsx";

    [Authorize(Roles = Roles.Admin + "," + Roles.Registrar + "," + Roles.Lecturer)]
    [HttpGet("options")]
    public Task<ReportOptionsDto> Options(CancellationToken ct) => Mediator.Send(new GetReportOptionsQuery(), ct);

    [Authorize(Roles = Roles.Admin + "," + Roles.Registrar)]
    [HttpGet("hall-timetable")]
    public async Task<IActionResult> HallTimetable([FromQuery] DateOnly from, [FromQuery] DateOnly to, [FromQuery] int? venueId,
        [FromQuery] string? programme, [FromQuery] int? academicYear, [FromQuery] string? format, CancellationToken ct) =>
        Respond(await Mediator.Send(new GetHallTimetableReportQuery(from, to, venueId, programme, academicYear), ct), format);

    [Authorize(Roles = Roles.Admin)]
    [HttpGet("event-attendance")]
    public async Task<IActionResult> EventAttendance([FromQuery] int? eventId, [FromQuery] string? format, CancellationToken ct) =>
        Respond(await Mediator.Send(new GetEventAttendanceReportQuery(eventId), ct), format);

    [Authorize(Roles = Roles.Admin)]
    [HttpGet("applications")]
    public async Task<IActionResult> Applications([FromQuery] DateOnly from, [FromQuery] DateOnly to, [FromQuery] ApplicationStatus? status,
        [FromQuery] string? format, CancellationToken ct) =>
        Respond(await Mediator.Send(new GetApplicationsReportQuery(from, to, status), ct), format);

    [Authorize(Roles = Roles.Admin + "," + Roles.Lecturer)]
    [HttpGet("student-participation")]
    public async Task<IActionResult> StudentParticipation([FromQuery] string? programme, [FromQuery] int? academicYear,
        [FromQuery] string? format, CancellationToken ct) =>
        Respond(await Mediator.Send(new GetStudentParticipationReportQuery(programme, academicYear), ct), format);

    private IActionResult Respond(ReportTable table, string? format)
    {
        if (!string.Equals(format, Excel, StringComparison.OrdinalIgnoreCase))
            return Ok(table);

        var file = writer.WriteExcel(table, clock.UtcNow);
        return File(file.Content, file.ContentType, file.FileName);
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Application.Features.Audit;
using NSBM.ICU.Application.Features.Audit.Queries.GetAuditLog;
using NSBM.ICU.Application.Features.Audit.Queries.GetAuditReport;
using NSBM.ICU.Domain.Constants;
using NSBM.ICU.Domain.Enums;

namespace NSBM.ICU.API.Controllers;

/// <summary>The Unit admin reads who did what. Read-only: entries are never edited or deleted.</summary>
[Authorize(Roles = Roles.Admin)]
[Route(ApiRoutes.V1 + "/audit")]
public sealed class AuditController(IReportFileWriter writer, IDateTime clock) : ApiControllerBase
{
    [HttpGet]
    public Task<AuditPageDto> List([FromQuery] AuditCategory? category, [FromQuery] string? search, [FromQuery] DateOnly? from,
        [FromQuery] DateOnly? to, [FromQuery] int page = 1, CancellationToken ct = default) =>
        Mediator.Send(new GetAuditLogQuery(new AuditFilter(category, search, from, to), page), ct);

    [HttpGet("export")]
    public async Task<IActionResult> Export([FromQuery] AuditCategory? category, [FromQuery] string? search, [FromQuery] DateOnly? from,
        [FromQuery] DateOnly? to, CancellationToken ct)
    {
        var table = await Mediator.Send(new GetAuditReportQuery(new AuditFilter(category, search, from, to)), ct);
        var file = writer.WriteExcel(table, clock.UtcNow);
        return File(file.Content, file.ContentType, file.FileName);
    }
}

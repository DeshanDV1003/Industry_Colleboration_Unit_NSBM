using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSBM.ICU.Application.Features.Analytics.Dtos;
using NSBM.ICU.Application.Features.Analytics.Queries.GetAnalytics;
using NSBM.ICU.Domain.Constants;

namespace NSBM.ICU.API.Controllers;

/// <summary>Aggregate trends for the Unit head and lecturers. No individual student is identified or ranked.</summary>
[Authorize(Roles = Roles.Admin + "," + Roles.Lecturer)]
[Route(ApiRoutes.V1 + "/analytics")]
public sealed class AnalyticsController : ApiControllerBase
{
    [HttpGet]
    public Task<AnalyticsDto> Get([FromQuery] int weeks = 12, CancellationToken ct = default) => Mediator.Send(new GetAnalyticsQuery(weeks), ct);
}

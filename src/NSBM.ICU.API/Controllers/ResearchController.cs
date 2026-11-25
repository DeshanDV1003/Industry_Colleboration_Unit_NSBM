using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSBM.ICU.Application.Features.Research.Dtos;
using NSBM.ICU.Application.Features.Research.Queries.GetResearchOverview;
using NSBM.ICU.Domain.Constants;

namespace NSBM.ICU.API.Controllers;

[Authorize(Roles = Roles.Lecturer + "," + Roles.Admin)]
[Route(ApiRoutes.V1 + "/research")]
public sealed class ResearchController : ApiControllerBase
{
    [HttpGet("overview")]
    public Task<ResearchOverviewDto> Overview(CancellationToken ct) => Mediator.Send(new GetResearchOverviewQuery(), ct);
}

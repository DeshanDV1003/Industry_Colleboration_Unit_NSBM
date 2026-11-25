using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSBM.ICU.Application.Features.Readiness.Dtos;
using NSBM.ICU.Application.Features.Readiness.Queries.GetReadiness;
using NSBM.ICU.Domain.Constants;

namespace NSBM.ICU.API.Controllers;

[Authorize(Roles = Roles.Student)]
[Route(ApiRoutes.V1 + "/me/readiness")]
public sealed class ReadinessController : ApiControllerBase
{
    /// <summary>Readiness for the chosen career path, or for one vacancy when <paramref name="vacancyId"/> is given.</summary>
    [HttpGet]
    public Task<ReadinessDto> Get([FromQuery] int? vacancyId, CancellationToken ct) =>
        Mediator.Send(new GetReadinessQuery(vacancyId), ct);
}

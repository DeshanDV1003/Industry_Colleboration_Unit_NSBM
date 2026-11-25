using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSBM.ICU.Application.Features.Careers.Commands.SelectCareerPath;
using NSBM.ICU.Application.Features.Careers.Dtos;
using NSBM.ICU.Application.Features.Careers.Queries.GetCareerPath;
using NSBM.ICU.Application.Features.Careers.Queries.GetCareerPaths;
using NSBM.ICU.Domain.Constants;

namespace NSBM.ICU.API.Controllers;

[Authorize]
[Route(ApiRoutes.V1 + "/career-paths")]
public sealed class CareerPathsController : ApiControllerBase
{
    [HttpGet]
    public Task<IReadOnlyList<CareerPathSummaryDto>> List(CancellationToken ct) =>
        Mediator.Send(new GetCareerPathsQuery(), ct);

    [HttpGet("{slug}")]
    public Task<CareerPathDetailDto> Get(string slug, CancellationToken ct) =>
        Mediator.Send(new GetCareerPathQuery(slug), ct);

    /// <summary>Chooses this path as the signed-in student's career goal.</summary>
    [Authorize(Roles = Roles.Student)]
    [HttpPost("{id:int}/selection")]
    public async Task<IActionResult> Select(int id, CancellationToken ct)
    {
        await Mediator.Send(new SelectCareerPathCommand(id), ct);
        return NoContent();
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSBM.ICU.Application.Features.Roadmap.Commands.SetRoadmapStepCompletion;
using NSBM.ICU.Application.Features.Roadmap.Dtos;
using NSBM.ICU.Application.Features.Roadmap.Queries.GetMyRoadmap;
using NSBM.ICU.Domain.Constants;

namespace NSBM.ICU.API.Controllers;

[Authorize(Roles = Roles.Student)]
[Route(ApiRoutes.V1 + "/me/roadmap")]
public sealed class RoadmapController : ApiControllerBase
{
    public sealed record StepCompletionRequest(bool Completed);

    [HttpGet]
    public Task<RoadmapDto> Get(CancellationToken ct) =>
        Mediator.Send(new GetMyRoadmapQuery(), ct);

    [HttpPut("steps/{stepId:int}/completion")]
    public async Task<IActionResult> SetCompletion(int stepId, StepCompletionRequest body, CancellationToken ct)
    {
        await Mediator.Send(new SetRoadmapStepCompletionCommand(stepId, body.Completed), ct);
        return NoContent();
    }
}

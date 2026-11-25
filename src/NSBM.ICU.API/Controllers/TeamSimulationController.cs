using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSBM.ICU.Application.Features.TeamSimulation.Commands.MoveSprintItem;
using NSBM.ICU.Application.Features.TeamSimulation.Dtos;
using NSBM.ICU.Application.Features.TeamSimulation.Queries.GetSprintBoard;
using NSBM.ICU.Domain.Constants;
using NSBM.ICU.Domain.Enums;

namespace NSBM.ICU.API.Controllers;

[Authorize(Roles = Roles.Student)]
[Route(ApiRoutes.V1 + "/me/sprint")]
public sealed class TeamSimulationController : ApiControllerBase
{
    public sealed record MoveRequest(SprintItemStatus Status);

    [HttpGet]
    public Task<SprintBoardDto> Board(CancellationToken ct) => Mediator.Send(new GetSprintBoardQuery(), ct);

    [HttpPut("items/{id:int}")]
    public async Task<IActionResult> Move(int id, MoveRequest body, CancellationToken ct)
    {
        await Mediator.Send(new MoveSprintItemCommand(id, body.Status), ct);
        return NoContent();
    }
}

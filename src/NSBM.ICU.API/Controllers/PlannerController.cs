using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSBM.ICU.Application.Features.Planner.Commands.AddPlannerItem;
using NSBM.ICU.Application.Features.Planner.Commands.DeletePlannerItem;
using NSBM.ICU.Application.Features.Planner.Commands.SetPlannerItemDone;
using NSBM.ICU.Application.Features.Planner.Commands.UpdatePlannerItem;
using NSBM.ICU.Application.Features.Planner.Dtos;
using NSBM.ICU.Application.Features.Planner.Models;
using NSBM.ICU.Application.Features.Planner.Queries.GetMyPlanner;
using NSBM.ICU.Domain.Constants;

namespace NSBM.ICU.API.Controllers;

[Authorize(Roles = Roles.Student)]
[Route(ApiRoutes.V1 + "/me/planner")]
public sealed class PlannerController : ApiControllerBase
{
    public sealed record DoneRequest(bool Done);

    [HttpGet]
    public Task<IReadOnlyList<PlannerItemDto>> List(CancellationToken ct) => Mediator.Send(new GetMyPlannerQuery(), ct);

    [HttpPost]
    public async Task<IActionResult> Add(PlannerItemInput item, CancellationToken ct) =>
        Ok(new { id = await Mediator.Send(new AddPlannerItemCommand(item), ct) });

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, PlannerItemInput item, CancellationToken ct)
    {
        await Mediator.Send(new UpdatePlannerItemCommand(id, item), ct);
        return NoContent();
    }

    [HttpPut("{id:int}/done")]
    public async Task<IActionResult> SetDone(int id, DoneRequest body, CancellationToken ct)
    {
        await Mediator.Send(new SetPlannerItemDoneCommand(id, body.Done), ct);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await Mediator.Send(new DeletePlannerItemCommand(id), ct);
        return NoContent();
    }
}

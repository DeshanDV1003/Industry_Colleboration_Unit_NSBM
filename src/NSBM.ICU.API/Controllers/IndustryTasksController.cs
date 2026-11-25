using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSBM.ICU.API.Controllers.Requests;
using NSBM.ICU.Application.Features.IndustryTasks.Commands.CreateIndustryTask;
using NSBM.ICU.Application.Features.IndustryTasks.Commands.ReviewIndustryTask;
using NSBM.ICU.Application.Features.IndustryTasks.Commands.UpdateIndustryTask;
using NSBM.ICU.Application.Features.IndustryTasks.Dtos;
using NSBM.ICU.Application.Features.IndustryTasks.Models;
using NSBM.ICU.Application.Features.IndustryTasks.Queries.GetCompanyTasks;
using NSBM.ICU.Application.Features.IndustryTasks.Queries.GetIndustryTask;
using NSBM.ICU.Application.Features.IndustryTasks.Queries.GetTaskReviewQueue;
using NSBM.ICU.Domain.Constants;

namespace NSBM.ICU.API.Controllers;

/// <summary>Authoring (industry partners) and moderation (Unit admins) of industry tasks.</summary>
[Authorize]
[Route(ApiRoutes.V1 + "/industry-tasks")]
public sealed class IndustryTasksController : ApiControllerBase
{
    [Authorize(Roles = Roles.Industry)]
    [HttpGet("mine")]
    public Task<IReadOnlyList<IndustryTaskListItemDto>> Mine(CancellationToken ct) =>
        Mediator.Send(new GetCompanyTasksQuery(), ct);

    [Authorize(Roles = Roles.Admin)]
    [HttpGet("review-queue")]
    public Task<IReadOnlyList<IndustryTaskListItemDto>> ReviewQueue(CancellationToken ct) =>
        Mediator.Send(new GetTaskReviewQueueQuery(), ct);

    [Authorize(Roles = Roles.Industry + "," + Roles.Admin)]
    [HttpGet("{id:int}")]
    public Task<IndustryTaskDto> Get(int id, CancellationToken ct) =>
        Mediator.Send(new GetIndustryTaskQuery(id), ct);

    [Authorize(Roles = Roles.Industry)]
    [HttpPost]
    public async Task<IActionResult> Create(IndustryTaskInput task, CancellationToken ct)
    {
        var id = await Mediator.Send(new CreateIndustryTaskCommand(task), ct);
        return CreatedAtAction(nameof(Get), new { id }, new { id });
    }

    [Authorize(Roles = Roles.Industry)]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, IndustryTaskInput task, CancellationToken ct)
    {
        await Mediator.Send(new UpdateIndustryTaskCommand(id, task), ct);
        return NoContent();
    }

    [Authorize(Roles = Roles.Admin)]
    [HttpPost("{id:int}/review")]
    public async Task<IActionResult> Review(int id, ReviewRequest body, CancellationToken ct)
    {
        await Mediator.Send(new ReviewIndustryTaskCommand(id, body.Decision, body.Note), ct);
        return NoContent();
    }
}

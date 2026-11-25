using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSBM.ICU.Application.Features.Practice.Commands.SubmitKnowledgeCheck;
using NSBM.ICU.Application.Features.Practice.Commands.SubmitTaskAttempt;
using NSBM.ICU.Application.Features.Practice.Dtos;
using NSBM.ICU.Application.Features.Practice.Queries.GetKnowledgeCheck;
using NSBM.ICU.Application.Features.Practice.Queries.GetTaskLibrary;
using NSBM.ICU.Application.Features.Practice.Queries.GetTaskWorkbench;
using NSBM.ICU.Domain.Constants;
using NSBM.ICU.Domain.Enums;

namespace NSBM.ICU.API.Controllers;

/// <summary>Students practising approved industry tasks.</summary>
[Authorize(Roles = Roles.Student)]
[Route(ApiRoutes.V1 + "/practice/tasks")]
public sealed class PracticeController : ApiControllerBase
{
    public sealed record AttemptRequest(string Code);
    public sealed record KnowledgeCheckRequest(IReadOnlyList<KnowledgeAnswerInput> Answers);

    [HttpGet]
    public Task<IReadOnlyList<TaskLibraryItemDto>> Library(
        [FromQuery] int? careerPathId, [FromQuery] LearningStage? level, [FromQuery] WorkSituation? situation, [FromQuery] string? search,
        CancellationToken ct) =>
        Mediator.Send(new GetTaskLibraryQuery(careerPathId, level, situation, search), ct);

    [HttpGet("{id:int}")]
    public Task<WorkbenchDto> Workbench(int id, CancellationToken ct) =>
        Mediator.Send(new GetTaskWorkbenchQuery(id), ct);

    [HttpPost("{id:int}/attempts")]
    public Task<TaskAttemptDto> SubmitAttempt(int id, AttemptRequest body, CancellationToken ct) =>
        Mediator.Send(new SubmitTaskAttemptCommand(id, body.Code), ct);

    [HttpGet("{id:int}/knowledge-check")]
    public Task<KnowledgeCheckDto> KnowledgeCheck(int id, CancellationToken ct) =>
        Mediator.Send(new GetKnowledgeCheckQuery(id), ct);

    [HttpPut("{id:int}/knowledge-check")]
    public async Task<IActionResult> SubmitKnowledgeCheck(int id, KnowledgeCheckRequest body, CancellationToken ct)
    {
        await Mediator.Send(new SubmitKnowledgeCheckCommand(id, body.Answers), ct);
        return NoContent();
    }
}

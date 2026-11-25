using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSBM.ICU.Application.Features.AiInterviews.Commands.AnswerAiInterview;
using NSBM.ICU.Application.Features.AiInterviews.Commands.ContinueAiInterview;
using NSBM.ICU.Application.Features.AiInterviews.Commands.FinishAiInterview;
using NSBM.ICU.Application.Features.AiInterviews.Commands.StartAiInterview;
using NSBM.ICU.Application.Features.AiInterviews.Dtos;
using NSBM.ICU.Application.Features.AiInterviews.Queries.GetAiInterview;
using NSBM.ICU.Application.Features.AiInterviews.Queries.GetAiInterviewAvailability;
using NSBM.ICU.Application.Features.AiInterviews.Queries.GetMyAiInterviews;
using NSBM.ICU.Domain.Constants;
using NSBM.ICU.Domain.Enums;

namespace NSBM.ICU.API.Controllers;

/// <summary>The student's private AI practice interviews. There is deliberately no lecturer or partner endpoint.</summary>
[Authorize(Roles = Roles.Student)]
[Route(ApiRoutes.V1 + "/me/ai-interviews")]
public sealed class AiInterviewsController : ApiControllerBase
{
    public sealed record StartRequest(InterviewStage Stage, bool ConsentToAiProcessing);
    public sealed record AnswerRequest(string Answer);

    [HttpGet("availability")]
    public Task<AiInterviewAvailabilityDto> Availability(CancellationToken ct) => Mediator.Send(new GetAiInterviewAvailabilityQuery(), ct);

    [HttpGet]
    public Task<IReadOnlyList<AiInterviewSummaryDto>> List(CancellationToken ct) => Mediator.Send(new GetMyAiInterviewsQuery(), ct);

    [HttpGet("{id:int}")]
    public Task<AiInterviewDto> Get(int id, CancellationToken ct) => Mediator.Send(new GetAiInterviewQuery(id), ct);

    [HttpPost]
    public Task<AiInterviewDto> Start(StartRequest body, CancellationToken ct) =>
        Mediator.Send(new StartAiInterviewCommand(body.Stage, body.ConsentToAiProcessing), ct);

    [HttpPost("{id:int}/answers")]
    public Task<AiInterviewDto> Answer(int id, AnswerRequest body, CancellationToken ct) => Mediator.Send(new AnswerAiInterviewCommand(id, body.Answer), ct);

    [HttpPost("{id:int}/continue")]
    public Task<AiInterviewDto> Continue(int id, CancellationToken ct) => Mediator.Send(new ContinueAiInterviewCommand(id), ct);

    [HttpPost("{id:int}/finish")]
    public Task<AiInterviewDto> Finish(int id, CancellationToken ct) => Mediator.Send(new FinishAiInterviewCommand(id), ct);
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSBM.ICU.Application.Features.Interviews.Commands.SubmitInterviewAnswer;
using NSBM.ICU.Application.Features.Interviews.Dtos;
using NSBM.ICU.Application.Features.Interviews.Queries.GetInterviewReport;
using NSBM.ICU.Application.Features.Interviews.Queries.GetInterviewSession;
using NSBM.ICU.Domain.Constants;
using NSBM.ICU.Domain.Enums;

namespace NSBM.ICU.API.Controllers;

/// <summary>Two-stage interview practice for students.</summary>
[Authorize(Roles = Roles.Student)]
[Route(ApiRoutes.V1 + "/me/interviews")]
public sealed class InterviewsController : ApiControllerBase
{
    [HttpGet("{stage}")]
    public Task<InterviewSessionDto> Session(InterviewStage stage, [FromQuery] int? careerPathId, CancellationToken ct) =>
        Mediator.Send(new GetInterviewSessionQuery(stage, careerPathId), ct);

    [HttpPost("answers")]
    public Task<AnswerFeedbackDto> Answer(SubmitInterviewAnswerCommand command, CancellationToken ct) =>
        Mediator.Send(command, ct);

    [HttpGet("report")]
    public Task<InterviewReportDto> Report(CancellationToken ct) =>
        Mediator.Send(new GetInterviewReportQuery(), ct);
}

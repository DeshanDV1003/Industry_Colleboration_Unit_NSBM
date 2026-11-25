using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSBM.ICU.Application.Features.Mentoring.Commands.AskMentor;
using NSBM.ICU.Application.Features.Mentoring.Commands.ReplyToMentorQuestion;
using NSBM.ICU.Application.Features.Mentoring.Dtos;
using NSBM.ICU.Application.Features.Mentoring.Queries.GetMentorQueue;
using NSBM.ICU.Application.Features.Mentoring.Queries.GetMyMentorQuestions;
using NSBM.ICU.Domain.Constants;

namespace NSBM.ICU.API.Controllers;

/// <summary>Students ask industry mentors; partners reply.</summary>
[Authorize]
[Route(ApiRoutes.V1 + "/mentoring")]
public sealed class MentoringController : ApiControllerBase
{
    public sealed record ReplyRequest(string Reply);

    [Authorize(Roles = Roles.Student)]
    [HttpGet("mine")]
    public Task<IReadOnlyList<MentorQuestionDto>> Mine(CancellationToken ct) => Mediator.Send(new GetMyMentorQuestionsQuery(), ct);

    [Authorize(Roles = Roles.Student)]
    [HttpPost("questions")]
    public async Task<IActionResult> Ask(AskMentorCommand command, CancellationToken ct) => Ok(new { id = await Mediator.Send(command, ct) });

    [Authorize(Roles = Roles.Industry)]
    [HttpGet("queue")]
    public Task<IReadOnlyList<MentorQuestionDto>> Queue(CancellationToken ct) => Mediator.Send(new GetMentorQueueQuery(), ct);

    [Authorize(Roles = Roles.Industry)]
    [HttpPost("questions/{id:int}/reply")]
    public async Task<IActionResult> Reply(int id, ReplyRequest body, CancellationToken ct)
    {
        await Mediator.Send(new ReplyToMentorQuestionCommand(id, body.Reply), ct);
        return NoContent();
    }
}

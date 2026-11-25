using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSBM.ICU.Application.Features.TaskReviews.Commands.ReviewTaskAttempt;
using NSBM.ICU.Application.Features.TaskReviews.Dtos;
using NSBM.ICU.Application.Features.TaskReviews.Queries.GetAttemptsForReview;
using NSBM.ICU.Domain.Constants;

namespace NSBM.ICU.API.Controllers;

/// <summary>Industry partners reviewing student attempts on their company's tasks.</summary>
[Authorize(Roles = Roles.Industry)]
[Route(ApiRoutes.V1 + "/task-reviews")]
public sealed class TaskReviewsController : ApiControllerBase
{
    public sealed record ReviewBody(string Comment, string? Improvements);

    [HttpGet("attempts")]
    public Task<IReadOnlyList<AttemptForReviewDto>> Attempts(CancellationToken ct) => Mediator.Send(new GetAttemptsForReviewQuery(), ct);

    [HttpPost("attempts/{attemptId:int}")]
    public async Task<IActionResult> Review(int attemptId, ReviewBody body, CancellationToken ct)
    {
        await Mediator.Send(new ReviewTaskAttemptCommand(attemptId, body.Comment, body.Improvements), ct);
        return NoContent();
    }
}

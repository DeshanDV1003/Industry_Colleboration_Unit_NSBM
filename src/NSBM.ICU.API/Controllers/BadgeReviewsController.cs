using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSBM.ICU.API.Controllers.Requests;
using NSBM.ICU.Application.Features.Achievements.Commands.ReviewSkillBadge;
using NSBM.ICU.Application.Features.Achievements.Dtos;
using NSBM.ICU.Application.Features.Achievements.Queries.GetBadgeReviewQueue;
using NSBM.ICU.Domain.Constants;

namespace NSBM.ICU.API.Controllers;

/// <summary>Lecturers review skill badge requests against the student's practice evidence.</summary>
[Authorize(Roles = Roles.Lecturer)]
[Route(ApiRoutes.V1 + "/badge-reviews")]
public sealed class BadgeReviewsController : ApiControllerBase
{
    [HttpGet]
    public Task<IReadOnlyList<BadgeReviewItemDto>> Queue(CancellationToken ct) => Mediator.Send(new GetBadgeReviewQueueQuery(), ct);

    [HttpPost("{id:int}")]
    public async Task<IActionResult> Review(int id, ReviewRequest body, CancellationToken ct)
    {
        await Mediator.Send(new ReviewSkillBadgeCommand(id, body.Decision, body.Note), ct);
        return NoContent();
    }
}

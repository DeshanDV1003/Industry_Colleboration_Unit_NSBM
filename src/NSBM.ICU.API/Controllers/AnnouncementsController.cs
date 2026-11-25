using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSBM.ICU.Application.Features.Announcements.Commands.SendAnnouncement;
using NSBM.ICU.Application.Features.Announcements.Dtos;
using NSBM.ICU.Application.Features.Announcements.Queries.GetAnnouncements;
using NSBM.ICU.Domain.Constants;

namespace NSBM.ICU.API.Controllers;

/// <summary>The Unit admin notifies everyone, or one group, at once.</summary>
[Authorize(Roles = Roles.Admin)]
[Route(ApiRoutes.V1 + "/announcements")]
public sealed class AnnouncementsController : ApiControllerBase
{
    [HttpGet]
    public Task<IReadOnlyList<AnnouncementDto>> List(CancellationToken ct) => Mediator.Send(new GetAnnouncementsQuery(), ct);

    [HttpPost]
    public async Task<IActionResult> Send(SendAnnouncementCommand body, CancellationToken ct) =>
        Ok(new { id = await Mediator.Send(body, ct) });
}

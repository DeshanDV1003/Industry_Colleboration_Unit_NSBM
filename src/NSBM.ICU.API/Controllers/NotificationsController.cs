using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSBM.ICU.Application.Features.Notifications.Commands.MarkNotificationsRead;
using NSBM.ICU.Application.Features.Notifications.Dtos;
using NSBM.ICU.Application.Features.Notifications.Queries.GetMyNotifications;
using NSBM.ICU.Application.Features.Notifications.Queries.GetWeeklyDigest;

namespace NSBM.ICU.API.Controllers;

[Authorize]
[Route(ApiRoutes.V1 + "/me")]
public sealed class NotificationsController : ApiControllerBase
{
    [HttpGet("notifications")]
    public Task<NotificationListDto> List([FromQuery] int take = 50, [FromQuery] bool unreadOnly = false, CancellationToken ct = default) =>
        Mediator.Send(new GetMyNotificationsQuery(take, unreadOnly), ct);

    [HttpPost("notifications/{id:int}/read")]
    public async Task<IActionResult> MarkRead(int id, CancellationToken ct)
    {
        await Mediator.Send(new MarkNotificationsReadCommand(id), ct);
        return NoContent();
    }

    [HttpPost("notifications/read-all")]
    public async Task<IActionResult> MarkAllRead(CancellationToken ct)
    {
        await Mediator.Send(new MarkNotificationsReadCommand(), ct);
        return NoContent();
    }

    [HttpGet("digest")]
    public Task<WeeklyDigestDto> Digest(CancellationToken ct) => Mediator.Send(new GetWeeklyDigestQuery(), ct);
}

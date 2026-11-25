using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSBM.ICU.Application.Features.Achievements.Commands.RequestSkillBadge;
using NSBM.ICU.Application.Features.Achievements.Commands.SetProfileCardSharing;
using NSBM.ICU.Application.Features.Achievements.Dtos;
using NSBM.ICU.Application.Features.Achievements.Queries.GetMyAchievements;
using NSBM.ICU.Domain.Constants;

namespace NSBM.ICU.API.Controllers;

/// <summary>The student's streak, level, skill badges and profile-card sharing.</summary>
[Authorize(Roles = Roles.Student)]
[Route(ApiRoutes.V1 + "/me/achievements")]
public sealed class AchievementsController : ApiControllerBase
{
    public sealed record BadgeRequest(int SkillId, string? Note);
    public sealed record SharingRequest(bool Share);

    [HttpGet]
    public Task<AchievementsDto> Get(CancellationToken ct) => Mediator.Send(new GetMyAchievementsQuery(), ct);

    [HttpPost("badges")]
    public async Task<IActionResult> RequestBadge(BadgeRequest body, CancellationToken ct) =>
        Ok(new { id = await Mediator.Send(new RequestSkillBadgeCommand(body.SkillId, body.Note), ct) });

    [HttpPut("profile-card")]
    public Task<ProfileCardSettingsDto> SetSharing(SharingRequest body, CancellationToken ct) =>
        Mediator.Send(new SetProfileCardSharingCommand(body.Share), ct);
}

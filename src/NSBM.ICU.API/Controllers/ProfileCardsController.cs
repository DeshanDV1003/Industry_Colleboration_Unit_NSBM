using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSBM.ICU.Application.Features.Achievements.Dtos;
using NSBM.ICU.Application.Features.Achievements.Queries.GetPublicProfileCard;

namespace NSBM.ICU.API.Controllers;

/// <summary>Public profile cards that students choose to share (for example on LinkedIn).</summary>
[AllowAnonymous]
[Route(ApiRoutes.V1 + "/profile-cards")]
public sealed class ProfileCardsController : ApiControllerBase
{
    [HttpGet("{slug}")]
    public Task<ProfileCardDto> Get(string slug, CancellationToken ct) => Mediator.Send(new GetPublicProfileCardQuery(slug), ct);
}

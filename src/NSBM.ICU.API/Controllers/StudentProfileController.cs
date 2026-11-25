using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSBM.ICU.Application.Features.StudentProfile.Commands.SetProfileSharing;
using NSBM.ICU.Application.Features.StudentProfile.Commands.UpdateMySkills;
using NSBM.ICU.Application.Features.StudentProfile.Dtos;
using NSBM.ICU.Application.Features.StudentProfile.Queries.GetMySkillProfile;
using NSBM.ICU.Domain.Constants;

namespace NSBM.ICU.API.Controllers;

/// <summary>The student's self-reported skills and profile-sharing choice.</summary>
[Authorize(Roles = Roles.Student)]
[Route(ApiRoutes.V1 + "/me/profile")]
public sealed class StudentProfileController : ApiControllerBase
{
    public sealed record SkillsRequest(IReadOnlyList<int> SkillIds);
    public sealed record SharingRequest(bool Share);

    [HttpGet("skills")]
    public Task<MySkillProfileDto> Get(CancellationToken ct) =>
        Mediator.Send(new GetMySkillProfileQuery(), ct);

    [HttpPut("skills")]
    public async Task<IActionResult> UpdateSkills(SkillsRequest body, CancellationToken ct)
    {
        await Mediator.Send(new UpdateMySkillsCommand(body.SkillIds), ct);
        return NoContent();
    }

    [HttpPut("sharing")]
    public async Task<IActionResult> SetSharing(SharingRequest body, CancellationToken ct)
    {
        await Mediator.Send(new SetProfileSharingCommand(body.Share), ct);
        return NoContent();
    }
}

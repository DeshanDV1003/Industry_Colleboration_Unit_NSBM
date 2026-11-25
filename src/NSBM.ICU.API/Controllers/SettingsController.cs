using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSBM.ICU.Application.Features.Settings.Commands.ResetDemoData;
using NSBM.ICU.Application.Features.Settings.Commands.SendTestEmail;
using NSBM.ICU.Application.Features.Settings.Commands.UpdateAdminProfile;
using NSBM.ICU.Domain.Constants;

namespace NSBM.ICU.API.Controllers;

[Authorize(Roles = Roles.Admin)]
[Route(ApiRoutes.V1 + "/settings")]
public sealed class SettingsController : ApiControllerBase
{
    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile(UpdateAdminProfileCommand command, CancellationToken ct)
    {
        await Mediator.Send(command, ct);
        return NoContent();
    }

    [HttpPost("demo-reset")]
    public async Task<IActionResult> ResetDemo(CancellationToken ct)
    {
        await Mediator.Send(new ResetDemoDataCommand(), ct);
        return NoContent();
    }

    /// <summary>Sends a test e-mail to the signed-in admin, to check the e-mail settings after a deployment.</summary>
    [HttpPost("test-email")]
    public async Task<IActionResult> TestEmail(CancellationToken ct) =>
        Ok(new { sentTo = await Mediator.Send(new SendTestEmailCommand(), ct) });
}

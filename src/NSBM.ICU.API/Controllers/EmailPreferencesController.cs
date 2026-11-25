using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSBM.ICU.Application.Features.EmailPreferences.Commands.UpdateMyEmailPreferences;
using NSBM.ICU.Application.Features.EmailPreferences.Dtos;
using NSBM.ICU.Application.Features.EmailPreferences.Queries.GetMyEmailPreferences;

namespace NSBM.ICU.API.Controllers;

/// <summary>Every signed-in person chooses which e-mails they receive.</summary>
[Authorize]
[Route(ApiRoutes.V1 + "/me/email-preferences")]
public sealed class EmailPreferencesController : ApiControllerBase
{
    [HttpGet]
    public Task<EmailPreferencesDto> Get(CancellationToken ct) => Mediator.Send(new GetMyEmailPreferencesQuery(), ct);

    [HttpPut]
    public async Task<IActionResult> Update(UpdateMyEmailPreferencesCommand command, CancellationToken ct)
    {
        await Mediator.Send(command, ct);
        return NoContent();
    }
}

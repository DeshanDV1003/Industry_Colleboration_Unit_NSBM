using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSBM.ICU.Application.Features.EventHub.Commands.CancelEventRegistration;
using NSBM.ICU.Application.Features.EventHub.Commands.CreateEventTeam;
using NSBM.ICU.Application.Features.EventHub.Commands.JoinEventTeam;
using NSBM.ICU.Application.Features.EventHub.Commands.LeaveEventTeam;
using NSBM.ICU.Application.Features.EventHub.Commands.RegisterForEvent;
using NSBM.ICU.Application.Features.EventHub.Dtos;
using NSBM.ICU.Application.Features.EventHub.Queries.GetEventForStudent;
using NSBM.ICU.Application.Features.EventHub.Queries.GetEventHub;
using NSBM.ICU.Application.Features.EventHub.Queries.GetMyCertificates;
using NSBM.ICU.Domain.Constants;

namespace NSBM.ICU.API.Controllers;

/// <summary>The student event hub: browse approved events, register, form teams, view certificates.</summary>
[Authorize(Roles = Roles.Student)]
[Route(ApiRoutes.V1 + "/events")]
public sealed class EventsController : ApiControllerBase
{
    public sealed record TeamRequest(string Name);
    public sealed record JoinRequest(string JoinCode);

    [HttpGet]
    public Task<IReadOnlyList<EventCardDto>> List(CancellationToken ct) => Mediator.Send(new GetEventHubQuery(), ct);

    [HttpGet("{id:int}")]
    public Task<EventDetailDto> Get(int id, CancellationToken ct) => Mediator.Send(new GetEventForStudentQuery(id), ct);

    [HttpGet("certificates")]
    public Task<IReadOnlyList<CertificateDto>> MyCertificates(CancellationToken ct) => Mediator.Send(new GetMyCertificatesQuery(), ct);

    [HttpPost("{id:int}/registration")]
    public async Task<IActionResult> Register(int id, CancellationToken ct)
    {
        await Mediator.Send(new RegisterForEventCommand(id), ct);
        return NoContent();
    }

    [HttpDelete("{id:int}/registration")]
    public async Task<IActionResult> Cancel(int id, CancellationToken ct)
    {
        await Mediator.Send(new CancelEventRegistrationCommand(id), ct);
        return NoContent();
    }

    [HttpPost("{id:int}/teams")]
    public async Task<IActionResult> CreateTeam(int id, TeamRequest body, CancellationToken ct) =>
        Ok(new { joinCode = await Mediator.Send(new CreateEventTeamCommand(id, body.Name), ct) });

    [HttpPost("{id:int}/teams/join")]
    public async Task<IActionResult> JoinTeam(int id, JoinRequest body, CancellationToken ct)
    {
        await Mediator.Send(new JoinEventTeamCommand(id, body.JoinCode), ct);
        return NoContent();
    }

    [HttpDelete("{id:int}/team")]
    public async Task<IActionResult> LeaveTeam(int id, CancellationToken ct)
    {
        await Mediator.Send(new LeaveEventTeamCommand(id), ct);
        return NoContent();
    }
}

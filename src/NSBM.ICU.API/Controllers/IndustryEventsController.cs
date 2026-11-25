using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSBM.ICU.API.Controllers.Requests;
using NSBM.ICU.Application.Features.IndustryEvents.Commands.CreateIndustryEvent;
using NSBM.ICU.Application.Features.IndustryEvents.Commands.IssueCertificates;
using NSBM.ICU.Application.Features.IndustryEvents.Commands.ReviewIndustryEvent;
using NSBM.ICU.Application.Features.IndustryEvents.Commands.SetAttendance;
using NSBM.ICU.Application.Features.IndustryEvents.Commands.UpdateIndustryEvent;
using NSBM.ICU.Application.Features.IndustryEvents.Dtos;
using NSBM.ICU.Application.Features.IndustryEvents.Models;
using NSBM.ICU.Application.Features.IndustryEvents.Queries.GetCompanyEvents;
using NSBM.ICU.Application.Features.IndustryEvents.Queries.GetEventAttendance;
using NSBM.ICU.Application.Features.IndustryEvents.Queries.GetEventReviewQueue;
using NSBM.ICU.Application.Features.IndustryEvents.Queries.GetIndustryEvent;
using NSBM.ICU.Domain.Constants;

namespace NSBM.ICU.API.Controllers;

/// <summary>Hosting (industry partners) and moderation (Unit admins) of industry events.</summary>
[Authorize]
[Route(ApiRoutes.V1 + "/industry-events")]
public sealed class IndustryEventsController : ApiControllerBase
{
    public sealed record AttendanceRequest(bool Attended);

    [Authorize(Roles = Roles.Industry)]
    [HttpGet("mine")]
    public Task<IReadOnlyList<IndustryEventListItemDto>> Mine(CancellationToken ct) => Mediator.Send(new GetCompanyEventsQuery(), ct);

    [Authorize(Roles = Roles.Admin)]
    [HttpGet("review-queue")]
    public Task<IReadOnlyList<IndustryEventListItemDto>> ReviewQueue(CancellationToken ct) => Mediator.Send(new GetEventReviewQueueQuery(), ct);

    [Authorize(Roles = Roles.Industry + "," + Roles.Admin)]
    [HttpGet("{id:int}")]
    public Task<IndustryEventDto> Get(int id, CancellationToken ct) => Mediator.Send(new GetIndustryEventQuery(id), ct);

    [Authorize(Roles = Roles.Industry)]
    [HttpPost]
    public async Task<IActionResult> Create(IndustryEventInput body, CancellationToken ct)
    {
        var id = await Mediator.Send(new CreateIndustryEventCommand(body), ct);
        return CreatedAtAction(nameof(Get), new { id }, new { id });
    }

    [Authorize(Roles = Roles.Industry)]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, IndustryEventInput body, CancellationToken ct)
    {
        await Mediator.Send(new UpdateIndustryEventCommand(id, body), ct);
        return NoContent();
    }

    [Authorize(Roles = Roles.Admin)]
    [HttpPost("{id:int}/review")]
    public async Task<IActionResult> Review(int id, ReviewRequest body, CancellationToken ct)
    {
        await Mediator.Send(new ReviewIndustryEventCommand(id, body.Decision, body.Note), ct);
        return NoContent();
    }

    [Authorize(Roles = Roles.Industry)]
    [HttpGet("{id:int}/attendance")]
    public Task<EventAttendanceDto> Attendance(int id, CancellationToken ct) => Mediator.Send(new GetEventAttendanceQuery(id), ct);

    [Authorize(Roles = Roles.Industry)]
    [HttpPut("{id:int}/attendance/{registrationId:int}")]
    public async Task<IActionResult> SetAttendance(int id, int registrationId, AttendanceRequest body, CancellationToken ct)
    {
        await Mediator.Send(new SetAttendanceCommand(id, registrationId, body.Attended), ct);
        return NoContent();
    }

    [Authorize(Roles = Roles.Industry)]
    [HttpPost("{id:int}/certificates")]
    public async Task<IActionResult> IssueCertificates(int id, CancellationToken ct) =>
        Ok(new { issued = await Mediator.Send(new IssueCertificatesCommand(id), ct) });
}

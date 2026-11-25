using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSBM.ICU.Application.Features.Calendar.Commands.CreateCalendarEvent;
using NSBM.ICU.Application.Features.Calendar.Commands.DeleteCalendarEvent;
using NSBM.ICU.Application.Features.Calendar.Commands.UpdateCalendarEvent;
using NSBM.ICU.Application.Features.Calendar.Dtos;
using NSBM.ICU.Application.Features.Calendar.Models;
using NSBM.ICU.Application.Features.Calendar.Queries.GetCalendarOptions;
using NSBM.ICU.Application.Features.Calendar.Queries.GetMyCalendar;
using NSBM.ICU.Domain.Constants;

namespace NSBM.ICU.API.Controllers;

/// <summary>Every role reads its own view of the year; only the Unit admin plans events.</summary>
[Authorize]
[Route(ApiRoutes.V1 + "/calendar")]
public sealed class CalendarController : ApiControllerBase
{
    [HttpGet("{year:int}")]
    public Task<IReadOnlyList<CalendarEntryDto>> Year(int year, CancellationToken ct) => Mediator.Send(new GetMyCalendarQuery(year), ct);

    [Authorize(Roles = Roles.Admin)]
    [HttpGet("options")]
    public Task<CalendarOptionsDto> Options(CancellationToken ct) => Mediator.Send(new GetCalendarOptionsQuery(), ct);

    [Authorize(Roles = Roles.Admin)]
    [HttpPost("events")]
    public async Task<IActionResult> Create(CalendarEventInput body, CancellationToken ct) =>
        Ok(new { id = await Mediator.Send(new CreateCalendarEventCommand(body), ct) });

    [Authorize(Roles = Roles.Admin)]
    [HttpPut("events/{id:int}")]
    public async Task<IActionResult> Update(int id, CalendarEventInput body, CancellationToken ct)
    {
        await Mediator.Send(new UpdateCalendarEventCommand(id, body), ct);
        return NoContent();
    }

    [Authorize(Roles = Roles.Admin)]
    [HttpDelete("events/{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await Mediator.Send(new DeleteCalendarEventCommand(id), ct);
        return NoContent();
    }
}

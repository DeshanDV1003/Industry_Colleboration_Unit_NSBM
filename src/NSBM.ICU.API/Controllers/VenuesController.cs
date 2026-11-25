using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSBM.ICU.Application.Features.Venues.Commands.CancelVenueBooking;
using NSBM.ICU.Application.Features.Venues.Commands.CreateVenue;
using NSBM.ICU.Application.Features.Venues.Commands.CreateVenueBooking;
using NSBM.ICU.Application.Features.Venues.Commands.UpdateVenue;
using NSBM.ICU.Application.Features.Venues.Dtos;
using NSBM.ICU.Application.Features.Venues.Models;
using NSBM.ICU.Application.Features.Venues.Queries.GetVenueBookingOptions;
using NSBM.ICU.Application.Features.Venues.Queries.GetVenueBookings;
using NSBM.ICU.Application.Features.Venues.Queries.GetVenues;
using NSBM.ICU.Domain.Constants;

namespace NSBM.ICU.API.Controllers;

/// <summary>Venues and hall allocations. The Assistant Registrar manages them; Unit admins can follow them.</summary>
[Authorize(Roles = Roles.Registrar + "," + Roles.Admin)]
[Route(ApiRoutes.V1 + "/venues")]
public sealed class VenuesController : ApiControllerBase
{
    [HttpGet]
    public Task<IReadOnlyList<VenueDto>> List(CancellationToken ct) => Mediator.Send(new GetVenuesQuery(), ct);

    [HttpGet("bookings")]
    public Task<IReadOnlyList<VenueBookingDto>> Bookings([FromQuery] DateOnly from, [FromQuery] DateOnly to, [FromQuery] int? venueId, CancellationToken ct) =>
        Mediator.Send(new GetVenueBookingsQuery(from, to, venueId), ct);

    [Authorize(Roles = Roles.Registrar)]
    [HttpGet("booking-options")]
    public Task<VenueBookingOptionsDto> BookingOptions(CancellationToken ct) => Mediator.Send(new GetVenueBookingOptionsQuery(), ct);

    [Authorize(Roles = Roles.Registrar)]
    [HttpPost]
    public async Task<IActionResult> Create(VenueInput body, CancellationToken ct) =>
        Ok(new { id = await Mediator.Send(new CreateVenueCommand(body), ct) });

    [Authorize(Roles = Roles.Registrar)]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, VenueInput body, CancellationToken ct)
    {
        await Mediator.Send(new UpdateVenueCommand(id, body), ct);
        return NoContent();
    }

    [Authorize(Roles = Roles.Registrar)]
    [HttpPost("bookings")]
    public async Task<IActionResult> Book(VenueBookingInput body, CancellationToken ct) =>
        Ok(new { ids = await Mediator.Send(new CreateVenueBookingCommand(body), ct) });

    [Authorize(Roles = Roles.Registrar)]
    [HttpDelete("bookings/{id:int}")]
    public async Task<IActionResult> Cancel(int id, CancellationToken ct)
    {
        await Mediator.Send(new CancelVenueBookingCommand(id), ct);
        return NoContent();
    }
}

using MediatR;
using Microsoft.EntityFrameworkCore;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Application.Features.Venues.Dtos;
using NSBM.ICU.Domain.Enums;

namespace NSBM.ICU.Application.Features.Venues.Queries.GetVenueBookingOptions;

/// <summary>The cohorts and the upcoming approved partner events and Unit events a hall can be allocated to.</summary>
public sealed record GetVenueBookingOptionsQuery : IRequest<VenueBookingOptionsDto>;

public sealed class GetVenueBookingOptionsQueryHandler(IApplicationDbContext db, IDateTime clock)
    : IRequestHandler<GetVenueBookingOptionsQuery, VenueBookingOptionsDto>
{
    public async Task<VenueBookingOptionsDto> Handle(GetVenueBookingOptionsQuery request, CancellationToken ct)
    {
        var today = DateOnly.FromDateTime(clock.UtcNow);

        var cohorts = await db.Students.AsNoTracking()
            .GroupBy(s => new { s.Programme, s.AcademicYear })
            .OrderBy(g => g.Key.Programme).ThenBy(g => g.Key.AcademicYear)
            .Select(g => new CohortOptionDto(g.Key.Programme, g.Key.AcademicYear, g.Count()))
            .ToListAsync(ct);

        var industryEvents = await db.IndustryEvents.AsNoTracking()
            .Where(e => e.Status == ModerationStatus.Approved && e.EndDate >= today)
            .OrderBy(e => e.StartDate)
            .Select(e => new EventOptionDto(e.Id, e.Title, e.Company!.Name, e.StartDate, e.EndDate, e.Capacity,
                db.VenueBookings.Any(b => b.IndustryEventId == e.Id)))
            .ToListAsync(ct);

        var unitEvents = await db.CalendarEvents.AsNoTracking()
            .Where(e => e.EndDate >= today)
            .OrderBy(e => e.StartDate)
            .Select(e => new EventOptionDto(e.Id, e.Title, null, e.StartDate, e.EndDate, null,
                db.VenueBookings.Any(b => b.CalendarEventId == e.Id)))
            .ToListAsync(ct);

        return new VenueBookingOptionsDto(cohorts, industryEvents, unitEvents);
    }
}

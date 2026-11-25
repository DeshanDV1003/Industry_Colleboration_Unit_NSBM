using MediatR;
using Microsoft.EntityFrameworkCore;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Application.Features.Venues.Dtos;

namespace NSBM.ICU.Application.Features.Venues.Queries.GetVenues;

/// <summary>Every venue with how many upcoming bookings it has. Active venues first.</summary>
public sealed record GetVenuesQuery : IRequest<IReadOnlyList<VenueDto>>;

public sealed class GetVenuesQueryHandler(IApplicationDbContext db, IDateTime clock) : IRequestHandler<GetVenuesQuery, IReadOnlyList<VenueDto>>
{
    public async Task<IReadOnlyList<VenueDto>> Handle(GetVenuesQuery request, CancellationToken ct)
    {
        var today = DateOnly.FromDateTime(clock.UtcNow);
        return await db.Venues.AsNoTracking()
            .OrderByDescending(v => v.IsActive).ThenBy(v => v.Code)
            .Select(v => new VenueDto(v.Id, v.Code, v.Name, v.Building, v.Type, v.Capacity, v.Facilities, v.IsActive,
                db.VenueBookings.Count(b => b.VenueId == v.Id && b.Date >= today)))
            .ToListAsync(ct);
    }
}

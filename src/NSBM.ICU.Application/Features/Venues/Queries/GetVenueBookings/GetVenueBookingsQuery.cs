using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Application.Features.Venues.Dtos;

namespace NSBM.ICU.Application.Features.Venues.Queries.GetVenueBookings;

/// <summary>The allocations in a date range, optionally for one venue, in time order.</summary>
public sealed record GetVenueBookingsQuery(DateOnly From, DateOnly To, int? VenueId) : IRequest<IReadOnlyList<VenueBookingDto>>;

public sealed class GetVenueBookingsQueryValidator : AbstractValidator<GetVenueBookingsQuery>
{
    public const int MaxRangeDays = 62;

    public GetVenueBookingsQueryValidator() =>
        RuleFor(x => x.To)
            .GreaterThanOrEqualTo(x => x.From).WithMessage("The end date cannot be before the start date.")
            .Must((x, to) => to.DayNumber - x.From.DayNumber <= MaxRangeDays).WithMessage($"Show at most {MaxRangeDays} days at a time.");
}

public sealed class GetVenueBookingsQueryHandler(IApplicationDbContext db, IDateTime clock)
    : IRequestHandler<GetVenueBookingsQuery, IReadOnlyList<VenueBookingDto>>
{
    public async Task<IReadOnlyList<VenueBookingDto>> Handle(GetVenueBookingsQuery request, CancellationToken ct)
    {
        var today = DateOnly.FromDateTime(clock.UtcNow);
        return await db.VenueBookings.AsNoTracking()
            .Where(b => b.Date >= request.From && b.Date <= request.To)
            .Where(b => request.VenueId == null || b.VenueId == request.VenueId)
            .OrderBy(b => b.Date).ThenBy(b => b.StartTime).ThenBy(b => b.Venue!.Code)
            .Select(b => new VenueBookingDto(b.Id, b.VenueId, b.Venue!.Code, b.Venue.Name, b.Purpose, b.Title, b.Date, b.StartTime,
                b.EndTime, b.Programme, b.AcademicYear, b.IndustryEventId, b.CalendarEventId, b.Attendees, b.Note, b.BookedByName,
                b.Date < today))
            .ToListAsync(ct);
    }
}

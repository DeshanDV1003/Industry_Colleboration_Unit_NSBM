using MediatR;
using Microsoft.EntityFrameworkCore;
using NSBM.ICU.Application.Common.Exceptions;
using NSBM.ICU.Application.Common.Extensions;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Domain.Enums;

namespace NSBM.ICU.Application.Features.Venues.Commands.CancelVenueBooking;

/// <summary>Frees a hall and tells everyone who was told about it. Past bookings are kept as a record.</summary>
public sealed record CancelVenueBookingCommand(int Id) : IRequest;

public sealed class CancelVenueBookingCommandHandler(IApplicationDbContext db, ICurrentUser currentUser, IDateTime clock) : IRequestHandler<CancelVenueBookingCommand>
{
    public async Task Handle(CancelVenueBookingCommand request, CancellationToken ct)
    {
        await db.GetCurrentRegistrarAsync(currentUser, ct);
        var booking = await db.VenueBookings.Include(b => b.Venue).FirstOrDefaultAsync(b => b.Id == request.Id, ct)
                      ?? throw NotFoundException.For("Booking", request.Id);

        if (booking.Date < DateOnly.FromDateTime(clock.UtcNow))
            throw new BusinessRuleException("Past bookings are kept as a record and cannot be cancelled.");

        db.NotifyAll(await db.ForAsync(booking, currentUser.UserId, ct), NotificationKind.HallBookingCancelled,
            $"Hall booking cancelled: {booking.Title}",
            $"{booking.Venue!.DisplayName} is no longer booked on {booking.Date:ddd d MMM yyyy}, {booking.StartTime:HH\\:mm}–{booking.EndTime:HH\\:mm}.",
            clock.UtcNow);

        db.VenueBookings.Remove(booking);
        await db.SaveChangesAsync(ct);
    }
}

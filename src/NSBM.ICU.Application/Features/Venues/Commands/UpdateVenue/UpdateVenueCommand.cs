using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NSBM.ICU.Application.Common.Exceptions;
using NSBM.ICU.Application.Common.Extensions;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Application.Features.Venues.Models;

namespace NSBM.ICU.Application.Features.Venues.Commands.UpdateVenue;

/// <summary>
/// Edits a venue or takes it out of use. Existing bookings stay; a smaller capacity cannot strand upcoming
/// bookings that would no longer fit.
/// </summary>
public sealed record UpdateVenueCommand(int Id, VenueInput Venue) : IRequest;

public sealed class UpdateVenueCommandValidator : AbstractValidator<UpdateVenueCommand>
{
    public UpdateVenueCommandValidator() => RuleFor(x => x.Venue).NotNull().SetValidator(new VenueInputValidator());
}

public sealed class UpdateVenueCommandHandler(IApplicationDbContext db, ICurrentUser currentUser, IDateTime clock) : IRequestHandler<UpdateVenueCommand>
{
    public async Task Handle(UpdateVenueCommand request, CancellationToken ct)
    {
        await db.GetCurrentRegistrarAsync(currentUser, ct);
        var venue = await db.Venues.FirstOrDefaultAsync(v => v.Id == request.Id, ct) ?? throw NotFoundException.For("Venue", request.Id);

        var code = request.Venue.Code.Trim().ToUpperInvariant();
        if (await db.Venues.AnyAsync(v => v.Id != venue.Id && v.Code == code, ct))
            throw new BusinessRuleException($"A venue with code {code} already exists.");

        var today = DateOnly.FromDateTime(clock.UtcNow);
        var largestUpcoming = await db.VenueBookings.Where(b => b.VenueId == venue.Id && b.Date >= today)
            .MaxAsync(b => (int?)b.Attendees, ct);
        if (largestUpcoming > request.Venue.Capacity)
            throw new BusinessRuleException($"An upcoming booking expects {largestUpcoming} people. Move it before lowering the capacity.");

        venue.Apply(request.Venue);
        await db.SaveChangesAsync(ct);
    }
}

using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NSBM.ICU.Application.Common.Auditing;
using NSBM.ICU.Application.Common.Exceptions;
using NSBM.ICU.Application.Common.Extensions;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Application.Features.Venues.Models;
using NSBM.ICU.Domain.Entities.Venues;
using NSBM.ICU.Domain.Enums;

namespace NSBM.ICU.Application.Features.Venues.Commands.CreateVenue;

/// <summary>The registrar adds a hall, lab or auditorium to the list of bookable venues.</summary>
public sealed record CreateVenueCommand(VenueInput Venue) : IRequest<int>;

public sealed class CreateVenueCommandValidator : AbstractValidator<CreateVenueCommand>
{
    public CreateVenueCommandValidator() => RuleFor(x => x.Venue).NotNull().SetValidator(new VenueInputValidator());
}

public sealed class CreateVenueCommandHandler(IApplicationDbContext db, ICurrentUser currentUser, IAuditTrail audit) : IRequestHandler<CreateVenueCommand, int>
{
    public async Task<int> Handle(CreateVenueCommand request, CancellationToken ct)
    {
        await db.GetCurrentRegistrarAsync(currentUser, ct);

        var venue = new Venue();
        venue.Apply(request.Venue);
        if (await db.Venues.AnyAsync(v => v.Code == venue.Code, ct))
            throw new BusinessRuleException($"A venue with code {venue.Code} already exists.");

        db.Venues.Add(venue);
        await db.SaveChangesAsync(ct);
        await audit.RecordAsync(AuditCategory.Halls, AuditActions.VenueCreated, $"Added the venue {venue.DisplayName} ({venue.Capacity} seats)",
            "Venue", venue.Id.ToString(), ct);
        await db.SaveChangesAsync(ct);
        return venue.Id;
    }
}

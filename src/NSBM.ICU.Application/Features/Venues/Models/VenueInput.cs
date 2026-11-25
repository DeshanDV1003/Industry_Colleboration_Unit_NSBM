using FluentValidation;
using NSBM.ICU.Domain.Entities.Venues;
using NSBM.ICU.Domain.Enums;

namespace NSBM.ICU.Application.Features.Venues.Models;

public sealed record VenueInput(
    string Code,
    string Name,
    string Building,
    VenueType Type,
    int Capacity,
    string? Facilities,
    bool IsActive);

public sealed class VenueInputValidator : AbstractValidator<VenueInput>
{
    public const int MaxCapacity = 2000;

    public VenueInputValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(20);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(120);
        RuleFor(x => x.Building).NotEmpty().MaximumLength(120);
        RuleFor(x => x.Type).IsInEnum();
        RuleFor(x => x.Capacity).InclusiveBetween(1, MaxCapacity);
        RuleFor(x => x.Facilities).MaximumLength(300);
    }
}

public static class VenueInputExtensions
{
    public static void Apply(this Venue venue, VenueInput input)
    {
        venue.Code = input.Code.Trim().ToUpperInvariant();
        venue.Name = input.Name.Trim();
        venue.Building = input.Building.Trim();
        venue.Type = input.Type;
        venue.Capacity = input.Capacity;
        venue.Facilities = string.IsNullOrWhiteSpace(input.Facilities) ? null : input.Facilities.Trim();
        venue.IsActive = input.IsActive;
    }
}

using FluentValidation;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Domain.Enums;

namespace NSBM.ICU.Application.Features.IndustryEvents.Models;

/// <summary>What a partner fills in to propose an event. Leave both team sizes empty for an individual event.</summary>
public sealed record IndustryEventInput(
    string Title,
    string Description,
    EventFormat Format,
    DateOnly StartDate,
    DateOnly EndDate,
    string Location,
    int Capacity,
    DateOnly RegistrationClosesOn,
    int? MinTeamSize,
    int? MaxTeamSize);

public sealed class IndustryEventInputValidator : AbstractValidator<IndustryEventInput>
{
    public const int MaxDurationDays = 14;
    public const int MaxCapacity = 1000;
    public const int MaxTeamSizeLimit = 10;

    public IndustryEventInputValidator(IDateTime clock)
    {
        var today = DateOnly.FromDateTime(clock.UtcNow);

        RuleFor(x => x.Title).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Description).NotEmpty().MinimumLength(20).WithMessage("Describe the event in at least 20 characters.").MaximumLength(4000);
        RuleFor(x => x.Format).IsInEnum();
        RuleFor(x => x.Location).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Capacity).InclusiveBetween(1, MaxCapacity);
        RuleFor(x => x.StartDate).GreaterThanOrEqualTo(today).WithMessage("The event must start today or later.");
        RuleFor(x => x.EndDate)
            .GreaterThanOrEqualTo(x => x.StartDate).WithMessage("The end date cannot be before the start date.")
            .Must((x, end) => end.DayNumber - x.StartDate.DayNumber < MaxDurationDays).WithMessage($"An event can last at most {MaxDurationDays} days.");
        RuleFor(x => x.RegistrationClosesOn)
            .GreaterThanOrEqualTo(today).WithMessage("Registration must close today or later.")
            .LessThanOrEqualTo(x => x.StartDate).WithMessage("Registration must close on or before the start date.");

        RuleFor(x => x.MaxTeamSize).NotNull().When(x => x.MinTeamSize is not null).WithMessage("Set both team sizes, or neither for an individual event.");
        RuleFor(x => x.MinTeamSize).NotNull().When(x => x.MaxTeamSize is not null).WithMessage("Set both team sizes, or neither for an individual event.");
        RuleFor(x => x.MinTeamSize).InclusiveBetween(1, MaxTeamSizeLimit).When(x => x.MinTeamSize is not null);
        RuleFor(x => x.MaxTeamSize)
            .InclusiveBetween(2, MaxTeamSizeLimit)
            .GreaterThanOrEqualTo(x => x.MinTeamSize ?? 0).WithMessage("The largest team size cannot be smaller than the smallest.")
            .When(x => x.MaxTeamSize is not null);
    }
}

using FluentValidation;
using NSBM.ICU.Domain.Enums;

namespace NSBM.ICU.Application.Features.Venues.Models;

/// <summary>
/// A hall allocation. A lecture names its cohort and can repeat weekly; an event booking links the event and
/// takes its title when none is given. Attendees default to the cohort size or event capacity.
/// </summary>
public sealed record VenueBookingInput(
    int VenueId,
    VenueBookingPurpose Purpose,
    string? Title,
    DateOnly Date,
    TimeOnly StartTime,
    TimeOnly EndTime,
    string? Programme,
    int? AcademicYear,
    int? IndustryEventId,
    int? CalendarEventId,
    int? ExpectedAttendees,
    string? Note,
    int RepeatWeeks = 1);

public sealed class VenueBookingInputValidator : AbstractValidator<VenueBookingInput>
{
    public const int MaxRepeatWeeks = 15;

    public VenueBookingInputValidator()
    {
        RuleFor(x => x.VenueId).GreaterThan(0);
        RuleFor(x => x.Purpose).IsInEnum();
        RuleFor(x => x.Title).MaximumLength(150);
        RuleFor(x => x.Note).MaximumLength(500);
        RuleFor(x => x.EndTime).GreaterThan(x => x.StartTime).WithMessage("The end time must be after the start time.");
        RuleFor(x => x.ExpectedAttendees).InclusiveBetween(1, VenueInputValidator.MaxCapacity).When(x => x.ExpectedAttendees is not null);
        RuleFor(x => x.RepeatWeeks).InclusiveBetween(1, MaxRepeatWeeks);

        When(x => x.Purpose == VenueBookingPurpose.Lecture, () =>
        {
            RuleFor(x => x.Title).NotEmpty().WithMessage("Enter the module or lecture name.");
            RuleFor(x => x.Programme).NotEmpty().WithMessage("Choose the cohort.").MaximumLength(150);
            RuleFor(x => x.AcademicYear).NotNull().WithMessage("Choose the cohort.").InclusiveBetween(1, 5);
            RuleFor(x => x.IndustryEventId).Null();
            RuleFor(x => x.CalendarEventId).Null();
        }).Otherwise(() =>
        {
            RuleFor(x => x.RepeatWeeks).Equal(1).WithMessage("Only lectures repeat weekly.");
            RuleFor(x => x.Programme).Null();
            RuleFor(x => x.AcademicYear).Null();
        });

        When(x => x.Purpose == VenueBookingPurpose.IndustryEvent, () =>
        {
            RuleFor(x => x.IndustryEventId).NotNull().WithMessage("Choose the partner event.");
            RuleFor(x => x.CalendarEventId).Null();
        });

        When(x => x.Purpose == VenueBookingPurpose.UnitEvent, () =>
        {
            RuleFor(x => x.CalendarEventId).NotNull().WithMessage("Choose the calendar event.");
            RuleFor(x => x.IndustryEventId).Null();
            RuleFor(x => x.ExpectedAttendees).NotNull().WithMessage("Enter how many people are expected.");
        });
    }
}

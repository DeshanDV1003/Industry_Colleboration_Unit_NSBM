using FluentValidation;
using NSBM.ICU.Domain.Enums;

namespace NSBM.ICU.Application.Features.Calendar.Models;

public sealed record CalendarEventInput(
    string Title,
    string? Description,
    CalendarEventType Type,
    CalendarAudience Audience,
    DateOnly StartDate,
    DateOnly EndDate,
    string? Location,
    int? CareerPathId,
    int? IndustryTaskId,
    int? VacancyId);

public sealed class CalendarEventInputValidator : AbstractValidator<CalendarEventInput>
{
    public const int MaxSpanDays = 92;

    public CalendarEventInputValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Description).MaximumLength(2000);
        RuleFor(x => x.Location).MaximumLength(150);
        RuleFor(x => x.Type).IsInEnum();
        RuleFor(x => x.Audience).IsInEnum();
        RuleFor(x => x.EndDate)
            .GreaterThanOrEqualTo(x => x.StartDate).WithMessage("The end date cannot be before the start date.")
            .Must((x, end) => end.DayNumber - x.StartDate.DayNumber <= MaxSpanDays).WithMessage($"An event can last at most {MaxSpanDays} days.");
        RuleFor(x => x.CareerPathId).Null().When(x => x.Audience != CalendarAudience.Students)
            .WithMessage("Only student events can target a career path.");
        RuleFor(x => x.IndustryTaskId).NotNull().When(x => x.Type == CalendarEventType.TaskRelease)
            .WithMessage("Choose the industry task being released.");
        RuleFor(x => x.IndustryTaskId).Null().When(x => x.Type != CalendarEventType.TaskRelease)
            .WithMessage("Only task releases link to an industry task.");
        RuleFor(x => x.VacancyId).Null()
            .When(x => x.Type is not (CalendarEventType.InterviewDrive or CalendarEventType.ApplicationWindow))
            .WithMessage("Only interview drives and application windows link to a vacancy.");
    }
}

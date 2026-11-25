namespace NSBM.ICU.Application.Features.Calendar.Models;

/// <summary>Where a calendar entry comes from: a planned Unit event or one of the viewer's own dates.</summary>
public enum CalendarEntrySource
{
    UnitEvent = 1,
    PlannerDeadline = 2,
    VacancyClosing = 3,
    AssignedTask = 4,
    InterviewInvite = 5,
    IndustryEvent = 6,
    HallBooking = 7,
}

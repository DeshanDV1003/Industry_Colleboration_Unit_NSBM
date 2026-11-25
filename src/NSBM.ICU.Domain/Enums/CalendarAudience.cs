namespace NSBM.ICU.Domain.Enums;

/// <summary>Who a Unit calendar event is meant for. The Unit admin always sees every event.</summary>
public enum CalendarAudience
{
    Everyone = 1,
    Students = 2,
    Lecturers = 3,
    Partners = 4,
}

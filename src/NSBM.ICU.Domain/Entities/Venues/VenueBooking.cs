using NSBM.ICU.Domain.Common;
using NSBM.ICU.Domain.Entities.Calendar;
using NSBM.ICU.Domain.Entities.Events;
using NSBM.ICU.Domain.Enums;

namespace NSBM.ICU.Domain.Entities.Venues;

/// <summary>
/// One allocation of a venue for a time slot on one day: a lecture for a cohort (programme and academic year),
/// a partner event or a Unit calendar event. Weekly lectures are stored as one booking per week.
/// </summary>
public class VenueBooking : AuditableEntity
{
    public int VenueId { get; set; }
    public Venue? Venue { get; set; }
    public VenueBookingPurpose Purpose { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateOnly Date { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }

    /// <summary>Set for lectures: the cohort the hall is allocated to.</summary>
    public string? Programme { get; set; }
    public int? AcademicYear { get; set; }
    public int? IndustryEventId { get; set; }
    public IndustryEvent? IndustryEvent { get; set; }
    public int? CalendarEventId { get; set; }
    public CalendarEvent? CalendarEvent { get; set; }

    public int Attendees { get; set; }
    public string? Note { get; set; }
    public string BookedByName { get; set; } = string.Empty;

    /// <summary>Two slots clash when they share a day and their times overlap; one ending as the next starts is fine.</summary>
    public bool ClashesWith(DateOnly date, TimeOnly start, TimeOnly end) =>
        Date == date && start < EndTime && StartTime < end;
}

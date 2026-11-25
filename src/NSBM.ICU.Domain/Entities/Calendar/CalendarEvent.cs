using NSBM.ICU.Domain.Common;
using NSBM.ICU.Domain.Constants;
using NSBM.ICU.Domain.Entities.Careers;
using NSBM.ICU.Domain.Entities.Opportunities;
using NSBM.ICU.Domain.Entities.Tasks;
using NSBM.ICU.Domain.Enums;

namespace NSBM.ICU.Domain.Entities.Calendar;

/// <summary>
/// An event or programme on the Unit's year calendar, planned by the Unit admin. It can target one career
/// path's students and link to an industry task (a task release) or a vacancy (an interview drive).
/// </summary>
public class CalendarEvent : AuditableEntity
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public CalendarEventType Type { get; set; }
    public CalendarAudience Audience { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public string? Location { get; set; }

    /// <summary>When set, only students on this career path (plus lecturers and admins) see the event.</summary>
    public int? CareerPathId { get; set; }
    public CareerPath? CareerPath { get; set; }
    public int? IndustryTaskId { get; set; }
    public IndustryTask? IndustryTask { get; set; }
    public int? VacancyId { get; set; }
    public Vacancy? Vacancy { get; set; }

    /// <summary>
    /// Lecturers also see student events so they can support their cohort. A career-path target narrows
    /// a student event to students on that path.
    /// </summary>
    public bool IsVisibleTo(string role, int? studentCareerPathId) => role switch
    {
        Roles.Admin or Roles.Registrar => true,
        Roles.Student => Audience is CalendarAudience.Everyone or CalendarAudience.Students
                         && (CareerPathId is null || CareerPathId == studentCareerPathId),
        Roles.Lecturer => Audience is CalendarAudience.Everyone or CalendarAudience.Students or CalendarAudience.Lecturers,
        Roles.Industry => Audience is CalendarAudience.Everyone or CalendarAudience.Partners,
        _ => false,
    };
}

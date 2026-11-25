namespace NSBM.ICU.Domain.Enums;

/// <summary>Who an admin announcement is sent to. Staff means lecturers, the registrar and Unit admins.</summary>
public enum AnnouncementAudience
{
    Everyone = 1,
    Students = 2,
    Lecturers = 3,
    Partners = 4,
    Staff = 5,
}

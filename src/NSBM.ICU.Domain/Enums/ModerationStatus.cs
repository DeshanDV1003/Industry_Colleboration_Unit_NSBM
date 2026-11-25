namespace NSBM.ICU.Domain.Enums;

/// <summary>Review state of partner-submitted content (tasks, vacancies). Only approved content reaches students.</summary>
public enum ModerationStatus
{
    Pending = 1,
    Approved = 2,
    Rejected = 3,
}

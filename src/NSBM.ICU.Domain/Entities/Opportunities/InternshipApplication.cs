using NSBM.ICU.Domain.Common;
using NSBM.ICU.Domain.Entities.People;
using NSBM.ICU.Domain.Enums;

namespace NSBM.ICU.Domain.Entities.Opportunities;

/// <summary>A student's practice application to a vacancy. The company moves it through its statuses.</summary>
public class InternshipApplication : AuditableEntity
{
    public int VacancyId { get; set; }
    public Vacancy? Vacancy { get; set; }
    public int StudentId { get; set; }
    public Student? Student { get; set; }

    public string CoverNote { get; set; } = string.Empty;
    public DateTime SubmittedAt { get; set; }
    /// <summary>The rule-based skill overlap when the student applied. A record only — not a hiring score.</summary>
    public int MatchPercentAtSubmission { get; set; }

    public ApplicationStatus Status { get; private set; } = ApplicationStatus.Submitted;
    public DateTime StatusChangedAt { get; private set; }

    public void Submit(DateTime at)
    {
        SubmittedAt = at;
        Status = ApplicationStatus.Submitted;
        StatusChangedAt = at;
    }

    /// <summary>Moves the application to a new status. Setting the current status again is a no-op.</summary>
    public void ChangeStatus(ApplicationStatus status, DateTime at)
    {
        if (Status == status) return;
        Status = status;
        StatusChangedAt = at;
    }
}

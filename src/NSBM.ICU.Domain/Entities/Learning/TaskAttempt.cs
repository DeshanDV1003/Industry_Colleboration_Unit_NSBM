using NSBM.ICU.Domain.Common;
using NSBM.ICU.Domain.Entities.People;
using NSBM.ICU.Domain.Entities.Tasks;

namespace NSBM.ICU.Domain.Entities.Learning;

/// <summary>One submission of code for an industry task, with the outcome of its pattern checks.</summary>
public class TaskAttempt : BaseEntity
{
    public int StudentId { get; set; }
    public Student? Student { get; set; }
    public int TaskId { get; set; }
    public IndustryTask? Task { get; set; }
    public int AttemptNumber { get; set; }
    public string Code { get; set; } = string.Empty;
    public DateTime SubmittedAt { get; set; }
    public int PassedChecks { get; set; }
    public int TotalChecks { get; set; }

    public ICollection<TaskCheckResult> CheckResults { get; } = [];
    public ICollection<TaskAttemptReview> Reviews { get; } = [];
}

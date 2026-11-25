using NSBM.ICU.Domain.Common;

namespace NSBM.ICU.Domain.Entities.Learning;

/// <summary>Review comments from the company that set the task, on one student attempt.</summary>
public class TaskAttemptReview : BaseEntity
{
    public int AttemptId { get; set; }
    public TaskAttempt? Attempt { get; set; }
    public int ReviewerPartnerId { get; set; }
    public string ReviewerName { get; set; } = string.Empty;
    public string Comment { get; set; } = string.Empty;
    public string? Improvements { get; set; }
    public DateTime ReviewedAt { get; set; }
}

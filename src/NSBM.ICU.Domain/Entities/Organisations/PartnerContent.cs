using NSBM.ICU.Domain.Common;
using NSBM.ICU.Domain.Enums;

namespace NSBM.ICU.Domain.Entities.Organisations;

/// <summary>
/// Content authored by an industry partner (tasks, vacancies). It reaches students only after the
/// Industry Collaboration Unit approves it; editing sends it back for review.
/// </summary>
public abstract class PartnerContent : AuditableEntity
{
    public int CompanyId { get; set; }
    public Company? Company { get; set; }
    public int AuthorPartnerId { get; set; }

    public ModerationStatus Status { get; private set; } = ModerationStatus.Pending;
    public DateTime? ReviewedAt { get; private set; }
    public string? ReviewedBy { get; private set; }
    public string? ReviewNote { get; private set; }

    /// <summary>Partners may change content only while it is waiting for review or after it was rejected.</summary>
    public bool CanBeEdited => Status != ModerationStatus.Approved;

    public bool CanBeReviewed => Status == ModerationStatus.Pending;

    public void Approve(string reviewer, DateTime at, string? note) => SetReview(ModerationStatus.Approved, reviewer, at, note);

    public void Reject(string reviewer, DateTime at, string note) => SetReview(ModerationStatus.Rejected, reviewer, at, note);

    /// <summary>Sends edited content back to the review queue.</summary>
    public void ResubmitForReview()
    {
        Status = ModerationStatus.Pending;
        ReviewedAt = null;
        ReviewedBy = null;
        ReviewNote = null;
    }

    private void SetReview(ModerationStatus status, string reviewer, DateTime at, string? note)
    {
        Status = status;
        ReviewedBy = reviewer;
        ReviewedAt = at;
        ReviewNote = string.IsNullOrWhiteSpace(note) ? null : note.Trim();
    }
}

using NSBM.ICU.Domain.Common;
using NSBM.ICU.Domain.Entities.Careers;
using NSBM.ICU.Domain.Enums;

namespace NSBM.ICU.Domain.Entities.Profiles;

/// <summary>
/// A skill badge a student asks for once the platform has practice evidence. A lecturer reviews the evidence and
/// approves or declines it; only approved badges appear on the student's profile card. One badge per skill.
/// </summary>
public class SkillBadge : AuditableEntity
{
    public int StudentId { get; set; }
    public int SkillId { get; set; }
    public Skill? Skill { get; set; }

    /// <summary>The practice evidence at the time of the request, so the reviewer sees what the student saw.</summary>
    public string EvidenceSummary { get; private set; } = string.Empty;
    public string? StudentNote { get; private set; }
    public DateTime RequestedAt { get; private set; }

    public ModerationStatus Status { get; private set; } = ModerationStatus.Pending;
    public string? ReviewerName { get; private set; }
    public string? ReviewNote { get; private set; }
    public DateTime? ReviewedAt { get; private set; }
    public string? VerificationCode { get; private set; }

    public static SkillBadge Request(int studentId, int skillId, string evidenceSummary, string? note, DateTime at)
    {
        var badge = new SkillBadge { StudentId = studentId, SkillId = skillId };
        badge.SetRequest(evidenceSummary, note, at);
        return badge;
    }

    /// <summary>A declined badge can be asked for again, for example after more practice.</summary>
    public void RequestAgain(string evidenceSummary, string? note, DateTime at)
    {
        if (Status != ModerationStatus.Rejected)
            throw new DomainRuleException(Status == ModerationStatus.Approved ? "You already hold this badge." : "This badge is already waiting for review.");
        SetRequest(evidenceSummary, note, at);
    }

    public void Approve(string reviewer, string code, string? note, DateTime at)
    {
        EnsurePending();
        Status = ModerationStatus.Approved;
        VerificationCode = code;
        SetReview(reviewer, note, at);
    }

    public void Decline(string reviewer, string note, DateTime at)
    {
        EnsurePending();
        if (string.IsNullOrWhiteSpace(note)) throw new DomainRuleException("Tell the student why, so they know what to practise.");
        Status = ModerationStatus.Rejected;
        SetReview(reviewer, note, at);
    }

    private void SetRequest(string evidenceSummary, string? note, DateTime at)
    {
        EvidenceSummary = evidenceSummary;
        StudentNote = string.IsNullOrWhiteSpace(note) ? null : note.Trim();
        RequestedAt = at;
        Status = ModerationStatus.Pending;
        ReviewerName = null;
        ReviewNote = null;
        ReviewedAt = null;
    }

    private void SetReview(string reviewer, string? note, DateTime at)
    {
        ReviewerName = reviewer;
        ReviewNote = string.IsNullOrWhiteSpace(note) ? null : note.Trim();
        ReviewedAt = at;
    }

    private void EnsurePending()
    {
        if (Status != ModerationStatus.Pending) throw new DomainRuleException("Only badges waiting for review can be reviewed.");
    }
}

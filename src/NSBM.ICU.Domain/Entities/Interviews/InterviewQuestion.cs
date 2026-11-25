using NSBM.ICU.Domain.Common;
using NSBM.ICU.Domain.Entities.Careers;
using NSBM.ICU.Domain.Enums;

namespace NSBM.ICU.Domain.Entities.Interviews;

/// <summary>
/// A practice interview question. Soft-skills questions are shared by every path; technical questions belong
/// to one career path. A question with a parent is a follow-up scenario for that parent.
/// </summary>
public class InterviewQuestion : BaseEntity
{
    public InterviewStage Stage { get; set; }
    public int? CareerPathId { get; set; }
    public CareerPath? CareerPath { get; set; }
    public int? ParentQuestionId { get; set; }
    public int SortOrder { get; set; }
    public string Topic { get; set; } = string.Empty;
    public string Prompt { get; set; } = string.Empty;
    /// <summary>Topic keywords a strong answer usually mentions, separated by semicolons.</summary>
    public string Keywords { get; set; } = string.Empty;
    public string Tip { get; set; } = string.Empty;

    public ICollection<InterviewQuestion> FollowUps { get; } = [];

    public IReadOnlyList<string> KeywordList =>
        Keywords.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
}

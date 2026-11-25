using NSBM.ICU.Domain.Common;
using NSBM.ICU.Domain.Entities.Interviews;
using NSBM.ICU.Domain.Enums;

namespace NSBM.ICU.Domain.Entities.Learning;

/// <summary>One practice answer. Students can answer again at any time; the latest answer is shown in the report.</summary>
public class InterviewAnswer : BaseEntity
{
    public int StudentId { get; set; }
    public int QuestionId { get; set; }
    public InterviewQuestion? Question { get; set; }
    public string Text { get; set; } = string.Empty;
    public AnswerInputMode InputMode { get; set; }
    public DateTime AnsweredAt { get; set; }
    /// <summary>Question keywords found in the answer, separated by semicolons.</summary>
    public string MatchedKeywords { get; set; } = string.Empty;
    public int WordCount { get; set; }
}

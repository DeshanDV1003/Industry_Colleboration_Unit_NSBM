using NSBM.ICU.Domain.Common;
using NSBM.ICU.Domain.Enums;

namespace NSBM.ICU.Domain.Entities.Interviews;

/// <summary>One message in an AI practice interview: a question from the interviewer or an answer from the student.</summary>
public class AiInterviewTurn : BaseEntity
{
    public int SessionId { get; set; }
    public int Order { get; set; }
    public AiTurnSpeaker Speaker { get; set; }
    public string Text { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

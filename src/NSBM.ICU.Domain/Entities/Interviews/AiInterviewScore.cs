using NSBM.ICU.Domain.Common;

namespace NSBM.ICU.Domain.Entities.Interviews;

/// <summary>An AI-generated practice score (1-5) for one rubric criterion, with a short reason.</summary>
public class AiInterviewScore : BaseEntity
{
    public int SessionId { get; set; }
    public string Criterion { get; set; } = string.Empty;
    public int Score { get; set; }
    public string Comment { get; set; } = string.Empty;
}

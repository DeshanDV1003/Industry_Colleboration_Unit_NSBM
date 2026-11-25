using NSBM.ICU.Domain.Common;

namespace NSBM.ICU.Domain.Entities.Tasks;

/// <summary>A follow-up question that asks the student to explain their work in their own words.</summary>
public class KnowledgeQuestion : BaseEntity
{
    public int TaskId { get; set; }
    public int SortOrder { get; set; }
    public string Prompt { get; set; } = string.Empty;
}

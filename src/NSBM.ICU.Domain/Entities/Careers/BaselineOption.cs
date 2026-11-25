using NSBM.ICU.Domain.Common;

namespace NSBM.ICU.Domain.Entities.Careers;

public class BaselineOption : BaseEntity
{
    public int QuestionId { get; set; }
    public int SortOrder { get; set; }
    public string Text { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
}

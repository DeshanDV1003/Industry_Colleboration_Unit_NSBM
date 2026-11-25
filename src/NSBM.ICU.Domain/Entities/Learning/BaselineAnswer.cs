using NSBM.ICU.Domain.Common;
using NSBM.ICU.Domain.Entities.Careers;

namespace NSBM.ICU.Domain.Entities.Learning;

public class BaselineAnswer : BaseEntity
{
    public int AttemptId { get; set; }
    public int QuestionId { get; set; }
    public BaselineQuestion? Question { get; set; }
    public int SelectedOptionId { get; set; }
    public bool IsCorrect { get; set; }
}

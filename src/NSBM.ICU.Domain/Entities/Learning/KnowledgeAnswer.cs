using NSBM.ICU.Domain.Common;
using NSBM.ICU.Domain.Entities.Tasks;

namespace NSBM.ICU.Domain.Entities.Learning;

public class KnowledgeAnswer : BaseEntity
{
    public int SubmissionId { get; set; }
    public int QuestionId { get; set; }
    public KnowledgeQuestion? Question { get; set; }
    public string Text { get; set; } = string.Empty;
}

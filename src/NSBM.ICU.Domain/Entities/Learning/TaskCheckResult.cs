using NSBM.ICU.Domain.Common;
using NSBM.ICU.Domain.Entities.Tasks;

namespace NSBM.ICU.Domain.Entities.Learning;

public class TaskCheckResult : BaseEntity
{
    public int AttemptId { get; set; }
    public int CheckId { get; set; }
    public TaskCheck? Check { get; set; }
    public bool Passed { get; set; }
}

using NSBM.ICU.Domain.Common;
using NSBM.ICU.Domain.Enums;

namespace NSBM.ICU.Domain.Entities.Planning;

/// <summary>A personal goal, task or deadline in the student's preparation planner.</summary>
public class PlannerItem : AuditableEntity
{
    public int StudentId { get; set; }
    public PlannerItemKind Kind { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public DateOnly? DueDate { get; set; }
    public bool IsDone { get; private set; }
    public DateTime? CompletedAt { get; private set; }

    public void SetDone(bool done, DateTime at)
    {
        if (IsDone == done) return;
        IsDone = done;
        CompletedAt = done ? at : null;
    }
}

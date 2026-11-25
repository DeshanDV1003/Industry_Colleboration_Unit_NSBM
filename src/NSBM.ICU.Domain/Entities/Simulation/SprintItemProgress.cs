using NSBM.ICU.Domain.Common;
using NSBM.ICU.Domain.Enums;

namespace NSBM.ICU.Domain.Entities.Simulation;

/// <summary>A student's own progress on a sprint item they picked up.</summary>
public class SprintItemProgress : BaseEntity
{
    public int StudentId { get; set; }
    public int WorkItemId { get; set; }
    public SprintWorkItem? WorkItem { get; set; }
    public SprintItemStatus Status { get; set; }
    public DateTime UpdatedAt { get; set; }
}

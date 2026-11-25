using NSBM.ICU.Domain.Common;
using NSBM.ICU.Domain.Entities.Careers;

namespace NSBM.ICU.Domain.Entities.Learning;

/// <summary>Records that a student marked a roadmap step as done.</summary>
public class RoadmapStepCompletion : BaseEntity
{
    public int StudentId { get; set; }
    public int RoadmapStepId { get; set; }
    public RoadmapStep? RoadmapStep { get; set; }
    public DateTime CompletedAt { get; set; }
}

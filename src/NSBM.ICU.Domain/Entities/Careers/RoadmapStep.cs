using NSBM.ICU.Domain.Common;
using NSBM.ICU.Domain.Enums;

namespace NSBM.ICU.Domain.Entities.Careers;

/// <summary>One ordered milestone in a career path's learning roadmap.</summary>
public class RoadmapStep : BaseEntity
{
    public int CareerPathId { get; set; }
    public int SortOrder { get; set; }
    public LearningStage Stage { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Outcome { get; set; }
    public int? SkillId { get; set; }
    public Skill? Skill { get; set; }
}

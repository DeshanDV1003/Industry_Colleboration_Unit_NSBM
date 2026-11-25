using NSBM.ICU.Domain.Common;

namespace NSBM.ICU.Domain.Entities.Careers;

/// <summary>An entry-level IT career role a student can prepare for (e.g. Software Engineering, QA).</summary>
public class CareerPath : BaseEntity
{
    public string Slug { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Tagline { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    /// <summary>Typical entry-level job titles, separated by semicolons.</summary>
    public string EntryRoles { get; set; } = string.Empty;
    public int SortOrder { get; set; }

    public ICollection<CareerPathSkill> Skills { get; } = [];
    public ICollection<RoadmapStep> RoadmapSteps { get; } = [];
    public ICollection<BaselineQuestion> BaselineQuestions { get; } = [];
}

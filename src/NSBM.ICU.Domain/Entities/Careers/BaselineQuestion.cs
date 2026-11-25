using NSBM.ICU.Domain.Common;

namespace NSBM.ICU.Domain.Entities.Careers;

/// <summary>A multiple-choice starter question that gives a first snapshot of a student's knowledge of one skill.</summary>
public class BaselineQuestion : BaseEntity
{
    public int CareerPathId { get; set; }
    public int SkillId { get; set; }
    public Skill? Skill { get; set; }
    public int SortOrder { get; set; }
    public string Prompt { get; set; } = string.Empty;
    public string? Explanation { get; set; }

    public ICollection<BaselineOption> Options { get; } = [];
}

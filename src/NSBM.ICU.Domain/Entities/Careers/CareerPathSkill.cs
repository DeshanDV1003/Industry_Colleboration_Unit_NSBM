using NSBM.ICU.Domain.Common;
using NSBM.ICU.Domain.Enums;

namespace NSBM.ICU.Domain.Entities.Careers;

/// <summary>A skill expected for a career path and how important it is.</summary>
public class CareerPathSkill : BaseEntity
{
    public int CareerPathId { get; set; }
    public int SkillId { get; set; }
    public Skill? Skill { get; set; }
    public SkillImportance Importance { get; set; }
}

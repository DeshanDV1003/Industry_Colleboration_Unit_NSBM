using NSBM.ICU.Domain.Common;
using NSBM.ICU.Domain.Entities.Careers;

namespace NSBM.ICU.Domain.Entities.Opportunities;

/// <summary>A skill a vacancy asks for, either required or preferred.</summary>
public class VacancySkill : BaseEntity
{
    public int VacancyId { get; set; }
    public int SkillId { get; set; }
    public Skill? Skill { get; set; }
    public bool IsRequired { get; set; }
}

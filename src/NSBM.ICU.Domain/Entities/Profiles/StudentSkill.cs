using NSBM.ICU.Domain.Common;
using NSBM.ICU.Domain.Entities.Careers;

namespace NSBM.ICU.Domain.Entities.Profiles;

/// <summary>A skill the student says they have (self-reported). It is a claim, not evidence.</summary>
public class StudentSkill : BaseEntity
{
    public int StudentId { get; set; }
    public int SkillId { get; set; }
    public Skill? Skill { get; set; }
    public DateTime AddedAt { get; set; }
}

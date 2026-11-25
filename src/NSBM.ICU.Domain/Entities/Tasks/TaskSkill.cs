using NSBM.ICU.Domain.Common;
using NSBM.ICU.Domain.Entities.Careers;

namespace NSBM.ICU.Domain.Entities.Tasks;

/// <summary>A tool or skill an industry task needs.</summary>
public class TaskSkill : BaseEntity
{
    public int TaskId { get; set; }
    public int SkillId { get; set; }
    public Skill? Skill { get; set; }
}

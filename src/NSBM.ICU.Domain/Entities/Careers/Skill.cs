using NSBM.ICU.Domain.Common;
using NSBM.ICU.Domain.Enums;

namespace NSBM.ICU.Domain.Entities.Careers;

/// <summary>A technical or professional skill referenced by career paths, tasks and vacancies.</summary>
public class Skill : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public SkillCategory Category { get; set; }
    public string? Description { get; set; }
}

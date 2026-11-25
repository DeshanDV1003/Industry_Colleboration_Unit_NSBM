using NSBM.ICU.Domain.Common;

namespace NSBM.ICU.Domain.Entities.Profiles;

/// <summary>The editable parts of a student's CV. Skills come from <see cref="StudentSkill"/>.</summary>
public class CvProfile : AuditableEntity
{
    public int StudentId { get; set; }
    public string Headline { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string Education { get; set; } = string.Empty;
    public string Projects { get; set; } = string.Empty;
    public string? GitHubUrl { get; set; }
    public string? LinkedInUrl { get; set; }
}

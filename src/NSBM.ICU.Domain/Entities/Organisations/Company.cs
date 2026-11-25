using NSBM.ICU.Domain.Common;

namespace NSBM.ICU.Domain.Entities.Organisations;

/// <summary>An industry partner organisation collaborating with the unit.</summary>
public class Company : AuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string Sector { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string? Website { get; set; }
    public string? Description { get; set; }
    public bool IsVerified { get; set; }

    public ICollection<People.IndustryPartner> Partners { get; } = [];
}

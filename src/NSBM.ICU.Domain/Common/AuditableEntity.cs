namespace NSBM.ICU.Domain.Common;

/// <summary>Entity whose creation and modification are stamped automatically by the persistence layer.</summary>
public abstract class AuditableEntity : BaseEntity
{
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
}

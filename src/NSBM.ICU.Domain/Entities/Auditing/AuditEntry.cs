using NSBM.ICU.Domain.Common;
using NSBM.ICU.Domain.Enums;

namespace NSBM.ICU.Domain.Entities.Auditing;

/// <summary>
/// One record of who did what and when. Entries are written once and never changed or deleted, so the
/// actor's name and role are copied in: the record stays readable after the account changes or is removed.
/// </summary>
public class AuditEntry : BaseEntity
{
    public DateTime At { get; init; }
    public string? ActorUserId { get; init; }
    public string ActorName { get; init; } = string.Empty;
    public string? ActorRole { get; init; }
    public AuditCategory Category { get; init; }
    /// <summary>A stable code such as "user.deactivated", for filtering and exports.</summary>
    public string Action { get; init; } = string.Empty;
    /// <summary>A plain sentence, e.g. "Deactivated Nimal Perera (Student)".</summary>
    public string Summary { get; init; } = string.Empty;
    public string? SubjectType { get; init; }
    public string? SubjectId { get; init; }
}

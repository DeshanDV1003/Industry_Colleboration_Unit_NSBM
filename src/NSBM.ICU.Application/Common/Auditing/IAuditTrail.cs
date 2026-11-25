using NSBM.ICU.Domain.Enums;

namespace NSBM.ICU.Application.Common.Auditing;

/// <summary>
/// Records what the signed-in person did. Like notifications, an entry is only added to the context: it commits in
/// the same save as the change it describes, so the log never claims something that did not happen.
/// Never put a password or other secret in a summary.
/// </summary>
public interface IAuditTrail
{
    Task RecordAsync(AuditCategory category, string action, string summary, string? subjectType = null, string? subjectId = null,
        CancellationToken cancellationToken = default);

    /// <summary>For actions taken while signed out (a forgotten-password reset): the account that acted is named explicitly.</summary>
    Task RecordForAsync(string actorUserId, AuditCategory category, string action, string summary, string? subjectType = null,
        string? subjectId = null, CancellationToken cancellationToken = default);
}

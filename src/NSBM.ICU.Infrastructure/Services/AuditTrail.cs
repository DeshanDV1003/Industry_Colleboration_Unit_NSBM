using Microsoft.EntityFrameworkCore;
using NSBM.ICU.Application.Common.Auditing;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Domain.Entities.Auditing;
using NSBM.ICU.Domain.Enums;
using NSBM.ICU.Infrastructure.Persistence;

namespace NSBM.ICU.Infrastructure.Services;

/// <summary>Adds audit entries for the signed-in person. The actor's name is looked up once per request.</summary>
public sealed class AuditTrail(ApplicationDbContext db, ICurrentUser currentUser, IDateTime clock) : IAuditTrail
{
    private string? _actorName;

    public async Task RecordAsync(AuditCategory category, string action, string summary, string? subjectType = null, string? subjectId = null,
        CancellationToken cancellationToken = default) =>
        Add(currentUser.UserId, await ActorNameAsync(cancellationToken), currentUser.Role, category, action, summary, subjectType, subjectId);

    public async Task RecordForAsync(string actorUserId, AuditCategory category, string action, string summary, string? subjectType = null,
        string? subjectId = null, CancellationToken cancellationToken = default)
    {
        var name = await db.Users.Where(u => u.Id == actorUserId).Select(u => u.FirstName + " " + u.LastName).FirstOrDefaultAsync(cancellationToken);
        var role = await (from userRole in db.UserRoles
                          join r in db.Roles on userRole.RoleId equals r.Id
                          where userRole.UserId == actorUserId
                          select r.Name).FirstOrDefaultAsync(cancellationToken);
        Add(actorUserId, string.IsNullOrWhiteSpace(name) ? "Unknown" : name.Trim(), role, category, action, summary, subjectType, subjectId);
    }

    private void Add(string? actorUserId, string actorName, string? actorRole, AuditCategory category, string action, string summary,
        string? subjectType, string? subjectId)
    {
        db.AuditEntries.Add(new AuditEntry
        {
            At = clock.UtcNow,
            ActorUserId = actorUserId,
            ActorName = actorName,
            ActorRole = actorRole,
            Category = category,
            Action = action,
            Summary = summary.Length <= 500 ? summary : summary[..499] + "…",
            SubjectType = subjectType,
            SubjectId = subjectId,
        });
    }

    private async Task<string> ActorNameAsync(CancellationToken ct)
    {
        if (_actorName is not null) return _actorName;
        var name = currentUser.UserId is { } id
            ? await db.Users.Where(u => u.Id == id).Select(u => u.FirstName + " " + u.LastName).FirstOrDefaultAsync(ct)
            : null;
        return _actorName = string.IsNullOrWhiteSpace(name) ? "Unknown" : name.Trim();
    }
}

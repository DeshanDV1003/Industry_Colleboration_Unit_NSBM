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
        CancellationToken cancellationToken = default)
    {
        db.AuditEntries.Add(new AuditEntry
        {
            At = clock.UtcNow,
            ActorUserId = currentUser.UserId,
            ActorName = await ActorNameAsync(cancellationToken),
            ActorRole = currentUser.Role,
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

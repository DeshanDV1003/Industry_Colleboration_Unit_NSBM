using Microsoft.EntityFrameworkCore;

namespace NSBM.ICU.Infrastructure.Persistence.Seed;

public static class SeedNotificationExtensions
{
    /// <summary>
    /// Sample notifications created by the seeders describe made-up history; they appear in the app but must never be
    /// e-mailed. Called only in demo mode, after demo seeding and after every demo reset, so real pending e-mails are never skipped.
    /// </summary>
    public static Task MarkSeededNotificationsEmailedAsync(this ApplicationDbContext db, CancellationToken ct) =>
        db.Notifications.Where(n => n.EmailHandledAt == null)
            .ExecuteUpdateAsync(n => n.SetProperty(x => x.EmailHandledAt, DateTime.UtcNow), ct);
}

using Microsoft.EntityFrameworkCore;
using NSBM.ICU.Domain.Entities.Calendar;
using NSBM.ICU.Domain.Enums;
using NSBM.ICU.Infrastructure.Persistence.Seed.Data;

namespace NSBM.ICU.Infrastructure.Persistence.Seed;

/// <summary>Loads the sample Unit year plan for the current year. Runs after tasks so releases can link to one.</summary>
public sealed class CalendarSeeder(ApplicationDbContext db)
{
    public async Task SeedAsync(CancellationToken cancellationToken)
    {
        if (await db.CalendarEvents.AnyAsync(cancellationToken))
            return;

        var year = DateTime.UtcNow.Year;
        var paths = await db.CareerPaths.ToDictionaryAsync(p => p.Slug, p => p.Id, cancellationToken);
        var approvedTasks = await db.IndustryTasks
            .Where(t => t.Status == ModerationStatus.Approved)
            .Select(t => new { t.Id, t.CareerPathId })
            .ToListAsync(cancellationToken);

        foreach (var seed in CalendarData.Events)
        {
            var start = new DateOnly(year, 1, 1).AddMonths(seed.Month - 1).AddDays(seed.Day - 1);
            int? pathId = seed.CareerPathSlug is null ? null : paths[seed.CareerPathSlug];
            int? taskId = !seed.ReleasesTask
                ? null
                : approvedTasks.FirstOrDefault(t => pathId is null || t.CareerPathId == pathId)?.Id;

            // A task release without an approved task to point at is skipped rather than seeded half-linked.
            if (seed.Type == CalendarEventType.TaskRelease && taskId is null)
                continue;

            db.CalendarEvents.Add(new CalendarEvent
            {
                Title = seed.Title,
                Description = seed.Description,
                Type = seed.Type,
                Audience = seed.Audience,
                StartDate = start,
                EndDate = start.AddDays(seed.Days - 1),
                Location = seed.Location,
                CareerPathId = pathId,
                IndustryTaskId = taskId,
            });
        }

        await db.SaveChangesAsync(cancellationToken);
    }
}

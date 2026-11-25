using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSBM.ICU.Domain.Constants;
using NSBM.ICU.Infrastructure.Identity;

namespace NSBM.ICU.Infrastructure.Persistence.Seed;

/// <summary>
/// Applies pending migrations and loads reference content (catalogue, interview questions, practice sprint) on every
/// start-up. Fictional demo people and records are loaded only when demo mode is on; a real deployment instead gets
/// its first Unit admin from the Bootstrap settings. Safe to run on every start-up.
/// </summary>
public sealed class DatabaseSeeder(
    ApplicationDbContext db,
    RoleManager<IdentityRole> roles,
    CatalogueSeeder catalogue,
    PeopleSeeder people,
    TaskSeeder tasks,
    OpportunitySeeder opportunities,
    InterviewSeeder interviews,
    SimulationSeeder simulation,
    EventSeeder events,
    AchievementSeeder achievements,
    CalendarSeeder calendar,
    VenueSeeder venues,
    BootstrapAdminSeeder bootstrapAdmin,
    IOptions<DemoOptions> demo,
    ILogger<DatabaseSeeder> logger)
{
    public async Task InitialiseAsync(CancellationToken cancellationToken = default)
    {
        await db.Database.MigrateAsync(cancellationToken);
        await SeedRolesAsync();
        await catalogue.SeedAsync(cancellationToken);
        await interviews.SeedAsync(cancellationToken);

        if (demo.Value.Enabled)
        {
            await people.SeedAsync(cancellationToken);
            await tasks.SeedAsync(cancellationToken);
            await opportunities.SeedAsync(cancellationToken);
        }

        // The sprint board is practice content for every deployment; its sample mentor question needs the demo cohort.
        await simulation.SeedAsync(cancellationToken);

        if (demo.Value.Enabled)
        {
            await events.SeedAsync(cancellationToken);
            await achievements.SeedAsync(cancellationToken);
            await calendar.SeedAsync(cancellationToken);
            await venues.SeedAsync(cancellationToken);
            await db.MarkSeededNotificationsEmailedAsync(cancellationToken);
        }

        await bootstrapAdmin.SeedAsync(cancellationToken);
        logger.LogInformation("Database is up to date. Demo data {State}.", demo.Value.Enabled ? "loaded" : "not loaded (demo mode is off)");
    }

    private async Task SeedRolesAsync()
    {
        foreach (var role in Roles.All)
        {
            if (!await roles.RoleExistsAsync(role))
                await roles.CreateAsync(new IdentityRole(role));
        }
    }
}

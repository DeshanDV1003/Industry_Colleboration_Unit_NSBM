using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSBM.ICU.Domain.Constants;

namespace NSBM.ICU.Infrastructure.Persistence.Seed;

/// <summary>Applies pending migrations and loads reference and demo data. Safe to run on every start-up.</summary>
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
    ILogger<DatabaseSeeder> logger)
{
    public async Task InitialiseAsync(CancellationToken cancellationToken = default)
    {
        await db.Database.MigrateAsync(cancellationToken);
        await SeedRolesAsync();
        await catalogue.SeedAsync(cancellationToken);
        await people.SeedAsync(cancellationToken);
        await tasks.SeedAsync(cancellationToken);
        await opportunities.SeedAsync(cancellationToken);
        await interviews.SeedAsync(cancellationToken);
        await simulation.SeedAsync(cancellationToken);
        await events.SeedAsync(cancellationToken);
        await achievements.SeedAsync(cancellationToken);
        await calendar.SeedAsync(cancellationToken);
        await venues.SeedAsync(cancellationToken);
        logger.LogInformation("Database is up to date and seeded.");
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

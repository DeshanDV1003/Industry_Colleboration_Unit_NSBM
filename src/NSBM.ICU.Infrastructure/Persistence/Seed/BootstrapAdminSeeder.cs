using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Application.Common.Models;
using NSBM.ICU.Domain.Constants;
using NSBM.ICU.Domain.Entities.People;
using NSBM.ICU.Infrastructure.Identity;

namespace NSBM.ICU.Infrastructure.Persistence.Seed;

/// <summary>
/// Creates the first Unit admin from <see cref="BootstrapOptions"/> when the database has none, so a fresh
/// production database can be signed into. That admin then creates every other account from the Users page.
/// </summary>
public sealed class BootstrapAdminSeeder(ApplicationDbContext db, IIdentityService identity, IOptions<BootstrapOptions> options,
    ILogger<BootstrapAdminSeeder> logger)
{
    public async Task SeedAsync(CancellationToken cancellationToken)
    {
        if (await db.Administrators.AnyAsync(cancellationToken))
            return;

        var bootstrap = options.Value;
        if (!bootstrap.IsConfigured)
        {
            logger.LogWarning("No Unit admin exists. Set Bootstrap__AdminEmail and Bootstrap__AdminPassword and restart to create the first one.");
            return;
        }

        var account = new NewAccount(Roles.Admin, bootstrap.AdminEmail.Trim().ToLowerInvariant(),
            Fallback(bootstrap.AdminFirstName, "Unit"), Fallback(bootstrap.AdminLastName, "Admin"), bootstrap.AdminPassword);
        await identity.CreateAccountAsync(account, new Administrator { Designation = bootstrap.AdminDesignation }, cancellationToken);
        logger.LogInformation("Created the first Unit admin {Email}. Remove the Bootstrap settings now.", account.Email);
    }

    private static string Fallback(string value, string fallback) => string.IsNullOrWhiteSpace(value) ? fallback : value.Trim();
}

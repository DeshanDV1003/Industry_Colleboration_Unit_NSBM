using Microsoft.Extensions.Diagnostics.HealthChecks;
using NSBM.ICU.Infrastructure.Persistence;

namespace NSBM.ICU.API.Services;

/// <summary>Ready only when the database answers, so a load balancer does not send traffic to an API that cannot serve it.</summary>
public sealed class DatabaseHealthCheck(ApplicationDbContext db) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default) =>
        await db.Database.CanConnectAsync(cancellationToken)
            ? HealthCheckResult.Healthy()
            : HealthCheckResult.Unhealthy("The database cannot be reached.");
}

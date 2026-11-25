using NSBM.ICU.Infrastructure.Identity;

namespace NSBM.ICU.API.Extensions;

/// <summary>
/// Refuses to start with settings that would be unsafe outside a developer's machine, and says exactly what to fix.
/// Runs before anything touches the database.
/// </summary>
public static class DeploymentChecks
{
    public const int MinSecretLength = 32;
    private const string DevelopmentSecretMarker = "local-development-only";

    public static void EnsureSafeSettings(this WebApplicationBuilder builder)
    {
        var config = builder.Configuration;
        var problems = new List<string>();

        var secret = config[$"{JwtOptions.SectionName}:{nameof(JwtOptions.Secret)}"] ?? string.Empty;
        if (secret.Length < MinSecretLength)
            problems.Add($"Jwt__Secret must be at least {MinSecretLength} random characters.");
        else if (!builder.Environment.IsDevelopment() && secret.Contains(DevelopmentSecretMarker, StringComparison.OrdinalIgnoreCase))
            problems.Add("Jwt__Secret is the development key. Generate a new random secret for this environment.");

        if (string.IsNullOrWhiteSpace(config.GetConnectionString("DefaultConnection")))
            problems.Add("ConnectionStrings__DefaultConnection is not set.");

        if (!builder.Environment.IsDevelopment() && (config.GetSection("AllowedOrigins").Get<string[]>() ?? []).Length == 0
            && config.GetValue<bool>("ServeWebOnSameOrigin") is false)
            problems.Add("AllowedOrigins is empty. List the web app's address (AllowedOrigins__0=https://...), or serve the web app from the same origin and set ServeWebOnSameOrigin=true.");

        if (problems.Count > 0)
            throw new InvalidOperationException("The API cannot start with these settings:\n - " + string.Join("\n - ", problems));
    }
}

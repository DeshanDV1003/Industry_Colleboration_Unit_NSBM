namespace NSBM.ICU.Infrastructure.Identity;

/// <summary>
/// The first Unit admin of a real (non-demo) deployment. Used once, when the database has no admin; set it with
/// environment variables (Bootstrap__AdminEmail, Bootstrap__AdminPassword, ...) and remove them after the first start.
/// </summary>
public sealed class BootstrapOptions
{
    public const string SectionName = "Bootstrap";

    public string AdminEmail { get; init; } = string.Empty;
    public string AdminPassword { get; init; } = string.Empty;
    public string AdminFirstName { get; init; } = string.Empty;
    public string AdminLastName { get; init; } = string.Empty;
    public string AdminDesignation { get; init; } = "Head, Industry Collaboration Unit";

    public bool IsConfigured => !string.IsNullOrWhiteSpace(AdminEmail) && !string.IsNullOrWhiteSpace(AdminPassword);
}

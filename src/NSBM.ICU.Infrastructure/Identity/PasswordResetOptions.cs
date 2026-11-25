namespace NSBM.ICU.Infrastructure.Identity;

/// <summary>How long an e-mailed reset link works. A link also stops working once used or after any password change.</summary>
public sealed class PasswordResetOptions
{
    public const string SectionName = "PasswordReset";

    public int LinkValidMinutes { get; init; } = 60;
}

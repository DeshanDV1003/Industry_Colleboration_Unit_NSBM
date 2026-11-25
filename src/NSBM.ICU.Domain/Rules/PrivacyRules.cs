namespace NSBM.ICU.Domain.Rules;

/// <summary>Rules that keep aggregate figures from describing one person.</summary>
public static class PrivacyRules
{
    /// <summary>
    /// A rate or average over fewer people than this would effectively be one person's own result, so it is withheld
    /// and only the head count is shown.
    /// </summary>
    public const int MinGroupSize = 3;

    public static bool CanShowRate(int groupSize) => groupSize >= MinGroupSize;
}

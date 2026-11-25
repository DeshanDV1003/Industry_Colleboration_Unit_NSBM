using NSBM.ICU.Domain.Enums;

namespace NSBM.ICU.Domain.Rules;

/// <summary>Simple, transparent rules for early-support flags. They help lecturers decide whom to contact — nothing more.</summary>
public static class SupportSignalRules
{
    public static readonly TimeSpan InactivityWindow = TimeSpan.FromDays(14);
    public const int LowBaselinePercent = 40;
    public const int StrugglingAttempts = 2;

    /// <param name="hasPractised">Any baseline, task attempt, knowledge check or interview answer.</param>
    /// <param name="lastActivityAt">The latest activity of any kind, including applications.</param>
    /// <param name="tasksStruggling">Tasks with at least <see cref="StrugglingAttempts"/> attempts and fewer than half the checks passed.</param>
    public static IReadOnlyList<SupportSignal> Evaluate(
        bool hasCareerPath, bool hasPractised, DateTime? lastActivityAt, DateTime now, int? baselinePercent, int tasksStruggling)
    {
        var signals = new List<SupportSignal>();
        if (!hasCareerPath) signals.Add(SupportSignal.NoCareerPath);
        if (!hasPractised) signals.Add(SupportSignal.NoPracticeYet);
        if (lastActivityAt is { } last && now - last > InactivityWindow) signals.Add(SupportSignal.Inactive);
        if (baselinePercent is < LowBaselinePercent) signals.Add(SupportSignal.LowBaseline);
        if (tasksStruggling > 0) signals.Add(SupportSignal.StrugglingWithTask);
        return signals;
    }

    public static bool IsStruggling(int attempts, int bestPassed, int totalChecks) =>
        attempts >= StrugglingAttempts && totalChecks > 0 && bestPassed * 2 < totalChecks;
}

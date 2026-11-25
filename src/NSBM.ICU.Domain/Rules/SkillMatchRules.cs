using NSBM.ICU.Domain.Enums;

namespace NSBM.ICU.Domain.Rules;

/// <summary>
/// Transparent rules for comparing a student's profile with the skills a role or vacancy asks for.
/// The match percentage counts overlapping required skills only. It does not measure CV quality,
/// promise an interview or predict hiring.
/// </summary>
public static class SkillMatchRules
{
    public static SkillEvidence Classify(bool listed, bool practised) =>
        practised ? SkillEvidence.PracticeSignal
        : listed ? SkillEvidence.ListedOnly
        : SkillEvidence.Missing;

    /// <summary>Share of required skills that appear in the profile (listed or practised).</summary>
    public static int MatchPercent(IReadOnlyCollection<SkillEvidence> requiredEvidence) =>
        SkillSignalRules.Percent(requiredEvidence.Count(e => e != SkillEvidence.Missing), requiredEvidence.Count);
}

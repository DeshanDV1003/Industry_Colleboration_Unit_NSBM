using NSBM.ICU.Domain.Enums;
using NSBM.ICU.Domain.Rules;

namespace NSBM.ICU.Application.Common.Evidence;

/// <summary>
/// What the platform knows about one student's skills: the skills they list, and practice signals
/// (with their sources) from baseline answers and industry tasks. Signals are indications, not proof.
/// </summary>
public sealed class StudentEvidence(IReadOnlySet<int> listedSkillIds, IReadOnlyDictionary<int, IReadOnlyList<string>> practiceSources)
{
    public SkillEvidence For(int skillId) =>
        SkillMatchRules.Classify(listedSkillIds.Contains(skillId), practiceSources.ContainsKey(skillId));

    public IReadOnlyList<string> SourcesFor(int skillId) =>
        practiceSources.TryGetValue(skillId, out var sources) ? sources : [];

    /// <summary>Describes each skill (in the given order) with its evidence and practice sources.</summary>
    public IReadOnlyList<SkillEvidenceDto> Describe(IEnumerable<(int Id, string Name)> skills) =>
        skills.Select(s => new SkillEvidenceDto(s.Id, s.Name, For(s.Id), SourcesFor(s.Id))).ToList();

    public static int MatchPercent(IReadOnlyCollection<SkillEvidenceDto> required) =>
        SkillMatchRules.MatchPercent(required.Select(r => r.Evidence).ToList());
}

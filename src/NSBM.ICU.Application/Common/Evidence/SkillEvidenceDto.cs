using NSBM.ICU.Domain.Enums;

namespace NSBM.ICU.Application.Common.Evidence;

/// <summary>One required or preferred skill compared with a student's profile.</summary>
public sealed record SkillEvidenceDto(int SkillId, string SkillName, SkillEvidence Evidence, IReadOnlyList<string> PracticeSources);

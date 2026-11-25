using NSBM.ICU.Application.Common.Evidence;

namespace NSBM.ICU.Application.Features.Readiness.Dtos;

/// <summary>
/// A student's profile compared with a target: their career path (core vs supporting skills) or one vacancy
/// (required vs preferred skills). <see cref="TargetName"/> is null when no path has been chosen yet.
/// </summary>
public sealed record ReadinessDto(
    string? TargetName,
    string? CompanyName,
    int? VacancyId,
    int MatchPercent,
    IReadOnlyList<SkillEvidenceDto> Required,
    IReadOnlyList<SkillEvidenceDto> Preferred);

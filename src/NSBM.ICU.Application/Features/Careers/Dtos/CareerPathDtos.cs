using NSBM.ICU.Domain.Enums;

namespace NSBM.ICU.Application.Features.Careers.Dtos;

public sealed record CareerPathSummaryDto(
    int Id,
    string Slug,
    string Name,
    string Tagline,
    IReadOnlyList<string> EntryRoles,
    IReadOnlyList<string> CoreSkills,
    int SkillCount,
    int RoadmapStepCount,
    bool IsSelected);

public sealed record CareerSkillDto(int SkillId, string Name, SkillCategory Category, SkillImportance Importance, string? Description);

public sealed record CareerPathDetailDto(
    int Id,
    string Slug,
    string Name,
    string Tagline,
    string Description,
    IReadOnlyList<string> EntryRoles,
    IReadOnlyList<CareerSkillDto> Skills,
    bool IsSelected);

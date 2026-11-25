using NSBM.ICU.Domain.Enums;

namespace NSBM.ICU.Application.Features.Roadmap.Dtos;

/// <summary>The student's roadmap. <see cref="CareerPathId"/> is null until a path is chosen.</summary>
public sealed record RoadmapDto(
    int? CareerPathId,
    string? CareerPathName,
    int CompletedCount,
    int TotalCount,
    int ProgressPercent,
    IReadOnlyList<RoadmapStepDto> Steps);

public sealed record RoadmapStepDto(
    int Id,
    int Order,
    LearningStage Stage,
    string Title,
    string Description,
    string? Outcome,
    string? SkillName,
    bool IsCompleted,
    DateTime? CompletedAt);

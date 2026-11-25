using NSBM.ICU.Application.Common.Models;
using NSBM.ICU.Domain.Enums;

namespace NSBM.ICU.Application.Features.IndustryTasks.Dtos;

public sealed record IndustryTaskListItemDto(
    int Id,
    string Title,
    string CompanyName,
    string CareerPathName,
    LearningStage Level,
    WorkSituation Situation,
    ModerationStatus Status,
    DateTime CreatedAt,
    DateTime? ReviewedAt,
    string? ReviewNote,
    int AttemptCount);

public sealed record TaskCheckDto(int Id, string Description, string Pattern, bool ShouldMatch, string? Hint);

public sealed record KnowledgeQuestionDto(int Id, string Prompt);

/// <summary>The complete task, as seen by its company and by the Unit's reviewers.</summary>
public sealed record IndustryTaskDto(
    int Id,
    string Title,
    string Summary,
    string Instructions,
    int CareerPathId,
    string CareerPathName,
    LearningStage Level,
    WorkSituation Situation,
    int EstimatedMinutes,
    string? StarterLanguage,
    string? StarterCode,
    string CompanyName,
    ModerationStatus Status,
    DateTime CreatedAt,
    DateTime? ReviewedAt,
    string? ReviewedBy,
    string? ReviewNote,
    bool CanBeEdited,
    bool CanBeReviewed,
    IReadOnlyList<SkillRefDto> Skills,
    IReadOnlyList<TaskCheckDto> Checks,
    IReadOnlyList<KnowledgeQuestionDto> KnowledgeQuestions);

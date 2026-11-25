using NSBM.ICU.Domain.Enums;

namespace NSBM.ICU.Application.Features.Practice.Dtos;

public sealed record TaskLibraryItemDto(
    int Id,
    string Title,
    string Summary,
    string CompanyName,
    int CareerPathId,
    string CareerPathName,
    LearningStage Level,
    WorkSituation Situation,
    int EstimatedMinutes,
    IReadOnlyList<string> Skills,
    int CheckCount,
    int MyAttemptCount,
    int? MyBestPassedChecks,
    bool KnowledgeCheckDone,
    string? AssignedBy);

public sealed record CheckSummaryDto(int Id, string Description);

public sealed record WorkbenchTaskDto(
    int Id,
    string Title,
    string Summary,
    string Instructions,
    string CompanyName,
    string CareerPathName,
    LearningStage Level,
    WorkSituation Situation,
    int EstimatedMinutes,
    string? StarterLanguage,
    string? StarterCode,
    IReadOnlyList<string> Skills,
    IReadOnlyList<CheckSummaryDto> Checks,
    int KnowledgeQuestionCount);

/// <summary>Outcome of one check. The hint is shown only for failed checks.</summary>
public sealed record CheckResultDto(int CheckId, string Description, bool Passed, string? Hint);

/// <summary>A review comment from the company that set the task.</summary>
public sealed record AttemptReviewDto(string ReviewerName, string Comment, string? Improvements, DateTime ReviewedAt);

public sealed record TaskAttemptDto(
    int Id,
    int AttemptNumber,
    DateTime SubmittedAt,
    int PassedChecks,
    int TotalChecks,
    string Code,
    IReadOnlyList<CheckResultDto> Results,
    IReadOnlyList<AttemptReviewDto> Reviews);

public sealed record WorkbenchDto(
    WorkbenchTaskDto Task,
    IReadOnlyList<TaskAttemptDto> Attempts,
    DateTime? KnowledgeCheckSubmittedAt);

public sealed record KnowledgeCheckQuestionDto(int Id, string Prompt, string? Answer);

public sealed record KnowledgeCheckDto(
    int TaskId,
    string TaskTitle,
    bool HasAttempt,
    DateTime? SubmittedAt,
    IReadOnlyList<KnowledgeCheckQuestionDto> Questions);

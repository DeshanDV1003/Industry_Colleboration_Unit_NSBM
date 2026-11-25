using NSBM.ICU.Domain.Enums;

namespace NSBM.ICU.Application.Features.Lecturing.Dtos;

public sealed record StudentDirectoryItemDto(
    int Id,
    string Name,
    string StudentNo,
    string Programme,
    string? CareerPathName,
    int TasksAttempted,
    int RoadmapPercent,
    DateTime? LastActivityAt,
    IReadOnlyList<SupportSignal> Signals);

public sealed record InsightAttemptDto(int TaskId, string TaskTitle, int Attempts, int BestPassedChecks, int TotalChecks, DateTime LastSubmittedAt, bool KnowledgeCheckDone);

public sealed record KnowledgeAnswerViewDto(string Prompt, string Answer);

public sealed record KnowledgeCheckViewDto(string TaskTitle, DateTime SubmittedAt, IReadOnlyList<KnowledgeAnswerViewDto> Answers);

public sealed record SupportNoteDto(int Id, string LecturerName, string Text, DateTime CreatedAt);

public sealed record TaskAssignmentDto(int Id, int TaskId, string TaskTitle, string LecturerName, string? Note, DateTime AssignedAt);

public sealed record ActivityItemDto(DateTime At, string Description);

/// <summary>Everything a lecturer needs to guide one student. For support and feedback, never for ranking.</summary>
public sealed record StudentInsightsDto(
    int Id,
    string Name,
    string StudentNo,
    string Programme,
    string Batch,
    string? CareerPathName,
    int? BaselinePercent,
    int RoadmapCompleted,
    int RoadmapTotal,
    IReadOnlyList<SupportSignal> Signals,
    IReadOnlyList<string> PractisedSkills,
    IReadOnlyList<string> ListedOnlySkills,
    IReadOnlyList<InsightAttemptDto> Attempts,
    IReadOnlyList<KnowledgeCheckViewDto> KnowledgeChecks,
    IReadOnlyList<TaskAssignmentDto> Assignments,
    IReadOnlyList<SupportNoteDto> Notes,
    IReadOnlyList<ActivityItemDto> RecentActivity);

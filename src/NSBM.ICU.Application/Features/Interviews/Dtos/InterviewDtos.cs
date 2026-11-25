using NSBM.ICU.Domain.Enums;

namespace NSBM.ICU.Application.Features.Interviews.Dtos;

/// <summary>Topic-based feedback on one answer. Simple keyword checks — not AI grading.</summary>
public sealed record AnswerFeedbackDto(
    int AnswerId,
    string Text,
    AnswerInputMode InputMode,
    DateTime AnsweredAt,
    int WordCount,
    IReadOnlyList<string> MatchedTopics,
    IReadOnlyList<string> TopicsToConsider,
    int CoveragePercent,
    IReadOnlyList<string> Notes);

public sealed record InterviewQuestionDto(
    int Id,
    string Topic,
    string Prompt,
    string Tip,
    AnswerFeedbackDto? LatestAnswer,
    IReadOnlyList<InterviewQuestionDto> FollowUps);

/// <summary>Questions for one stage. For the technical stage, <see cref="CareerPathId"/> is null until a path is chosen.</summary>
public sealed record InterviewSessionDto(
    InterviewStage Stage,
    int? CareerPathId,
    string? CareerPathName,
    IReadOnlyList<InterviewQuestionDto> Questions);

public sealed record ReportItemDto(InterviewStage Stage, string? CareerPathName, string Topic, string Prompt, bool IsFollowUp, AnswerFeedbackDto Answer);

public sealed record InterviewReportDto(string StudentName, string StudentNo, DateTime GeneratedAt, IReadOnlyList<ReportItemDto> Items);

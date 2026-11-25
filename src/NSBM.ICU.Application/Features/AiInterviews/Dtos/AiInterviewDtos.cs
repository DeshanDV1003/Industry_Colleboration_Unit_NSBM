using NSBM.ICU.Domain.Enums;

namespace NSBM.ICU.Application.Features.AiInterviews.Dtos;

public sealed record AiInterviewAvailabilityDto(bool IsAvailable, int QuestionsPerSession);

public sealed record AiInterviewSummaryDto(
    int Id,
    InterviewStage Stage,
    string TargetRole,
    AiInterviewStatus Status,
    DateTime StartedAt,
    DateTime? CompletedAt,
    int AnswerCount,
    double? AverageScore);

public sealed record AiInterviewTurnDto(int Order, AiTurnSpeaker Speaker, string Text, DateTime CreatedAt);

public sealed record AiInterviewScoreDto(string Criterion, int Score, string Comment);

/// <summary>AI-generated practice feedback. It describes the answers, not the person, and is never shared.</summary>
public sealed record AiInterviewFeedbackDto(
    string Summary,
    IReadOnlyList<string> Strengths,
    IReadOnlyList<string> Improvements,
    IReadOnlyList<AiInterviewScoreDto> Scores,
    double AverageScore);

/// <param name="AwaitingAnswer">The last message is an unanswered question.</param>
/// <param name="NeedsNextStep">The last message is an answer and the AI has not replied yet (for example after a service error) — call continue.</param>
public sealed record AiInterviewDto(
    int Id,
    InterviewStage Stage,
    string TargetRole,
    AiInterviewStatus Status,
    DateTime StartedAt,
    DateTime? CompletedAt,
    int QuestionsPerSession,
    int QuestionCount,
    bool AwaitingAnswer,
    bool NeedsNextStep,
    IReadOnlyList<AiInterviewTurnDto> Turns,
    AiInterviewFeedbackDto? Feedback);

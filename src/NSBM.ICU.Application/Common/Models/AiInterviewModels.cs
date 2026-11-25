using NSBM.ICU.Domain.Enums;

namespace NSBM.ICU.Application.Common.Models;

/// <summary>What the AI interviewer needs to know about the practice session.</summary>
/// <param name="QuestionNumber">The number of the question about to be asked (1-based).</param>
public sealed record AiInterviewBrief(
    InterviewStage Stage,
    string TargetRole,
    IReadOnlyList<string> FocusSkills,
    IReadOnlyList<string> Criteria,
    int QuestionNumber,
    int TotalQuestions);

public sealed record AiTranscriptLine(AiTurnSpeaker Speaker, string Text);

public sealed record AiCriterionScore(string Criterion, int Score, string Comment);

public sealed record AiInterviewEvaluation(
    string Summary,
    IReadOnlyList<string> Strengths,
    IReadOnlyList<string> Improvements,
    IReadOnlyList<AiCriterionScore> Scores);

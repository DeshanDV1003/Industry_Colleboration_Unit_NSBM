using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Application.Common.Models;

namespace NSBM.ICU.UnitTests.AiInterviews;

/// <summary>Stands in for Claude: records what it was asked and replies predictably.</summary>
internal sealed class FakeAiInterviewer : IAiInterviewer
{
    public List<(AiInterviewBrief Brief, IReadOnlyList<AiTranscriptLine> Transcript)> QuestionCalls { get; } = [];
    public List<IReadOnlyList<AiTranscriptLine>> EvaluationCalls { get; } = [];
    public bool IsAvailable => true;

    public Task<string> NextQuestionAsync(AiInterviewBrief brief, IReadOnlyList<AiTranscriptLine> transcript, CancellationToken ct)
    {
        QuestionCalls.Add((brief, transcript.ToList()));
        return Task.FromResult($"  Question {brief.QuestionNumber}?  ");
    }

    public Task<AiInterviewEvaluation> EvaluateAsync(AiInterviewBrief brief, IReadOnlyList<AiTranscriptLine> transcript, CancellationToken ct)
    {
        EvaluationCalls.Add(transcript.ToList());
        return Task.FromResult(new AiInterviewEvaluation(
            "Solid answers.",
            [" Clear examples ", ""],
            ["Quantify results"],
            brief.Criteria.Select((c, i) => new AiCriterionScore(c, i == 0 ? 9 : 3, "Comment.")).ToList()));
    }
}

internal sealed class FixedClock(DateTime now) : IDateTime
{
    public DateTime UtcNow { get; } = now;
}

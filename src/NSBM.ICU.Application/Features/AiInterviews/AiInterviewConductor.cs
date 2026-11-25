using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Application.Common.Models;
using NSBM.ICU.Domain.Entities.Interviews;
using NSBM.ICU.Domain.Enums;
using NSBM.ICU.Domain.Rules;

namespace NSBM.ICU.Application.Features.AiInterviews;

/// <summary>
/// Moves a session forward with the AI: asks the next question, or scores the session once every question is
/// answered. Shared by start, answer, continue and finish so the rules live in one place.
/// </summary>
public sealed class AiInterviewConductor(IAiInterviewer interviewer, IDateTime clock)
{
    /// <summary>Asks the next question, or completes the session when all questions have been answered.</summary>
    public async Task AdvanceAsync(AiInterviewSession session, CancellationToken ct)
    {
        if (session.HasAskedAllQuestions && !session.AwaitingAnswer)
        {
            await CompleteAsync(session, ct);
            return;
        }

        var question = await interviewer.NextQuestionAsync(BriefFor(session), Transcript(session), ct);
        session.AskQuestion(question.Trim(), clock.UtcNow);
    }

    public async Task CompleteAsync(AiInterviewSession session, CancellationToken ct)
    {
        var evaluation = await interviewer.EvaluateAsync(BriefFor(session), Transcript(session), ct);
        session.Complete(
            evaluation.Summary.Trim(),
            evaluation.Strengths.Select(x => x.Trim()).Where(x => x.Length > 0),
            evaluation.Improvements.Select(x => x.Trim()).Where(x => x.Length > 0),
            evaluation.Scores.Select(x => (x.Criterion, x.Score, x.Comment.Trim())).ToList(),
            clock.UtcNow);
    }

    private static AiInterviewBrief BriefFor(AiInterviewSession s) => new(
        s.Stage,
        s.TargetRole,
        s.FocusSkills.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries),
        AiInterviewRubric.CriteriaFor(s.Stage),
        s.QuestionCount + 1,
        AiInterviewRubric.QuestionsPerSession);

    private static List<AiTranscriptLine> Transcript(AiInterviewSession s) =>
        s.Turns.OrderBy(t => t.Order).Select(t => new AiTranscriptLine(t.Speaker, t.Text)).ToList();
}

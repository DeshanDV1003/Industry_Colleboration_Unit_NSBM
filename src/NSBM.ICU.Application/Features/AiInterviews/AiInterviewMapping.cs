using Microsoft.EntityFrameworkCore;
using NSBM.ICU.Application.Common.Exceptions;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Application.Features.AiInterviews.Dtos;
using NSBM.ICU.Domain.Entities.Interviews;
using NSBM.ICU.Domain.Enums;
using NSBM.ICU.Domain.Rules;

namespace NSBM.ICU.Application.Features.AiInterviews;

internal static class AiInterviewMapping
{
    /// <summary>Loads one of the student's own sessions with its turns and scores. Another student's id is simply not found.</summary>
    public static async Task<AiInterviewSession> LoadOwnSessionAsync(this IApplicationDbContext db, int sessionId, int studentId, CancellationToken ct) =>
        await db.AiInterviewSessions
            .Include(s => s.Turns)
            .Include(s => s.Scores)
            .FirstOrDefaultAsync(s => s.Id == sessionId && s.StudentId == studentId, ct)
        ?? throw NotFoundException.For("Practice interview", sessionId);

    public static AiInterviewDto ToDto(this AiInterviewSession s)
    {
        var turns = s.Turns.OrderBy(t => t.Order).Select(t => new AiInterviewTurnDto(t.Order, t.Speaker, t.Text, t.CreatedAt)).ToList();
        return new AiInterviewDto(s.Id, s.Stage, s.TargetRole, s.Status, s.StartedAt, s.CompletedAt, AiInterviewRubric.QuestionsPerSession,
            s.QuestionCount, s.AwaitingAnswer, s.AwaitingInterviewer, turns, s.Status == AiInterviewStatus.Completed ? ToFeedback(s) : null);
    }

    private static AiInterviewFeedbackDto ToFeedback(AiInterviewSession s)
    {
        var criteria = AiInterviewRubric.CriteriaFor(s.Stage).ToList();
        var scores = s.Scores.OrderBy(x => criteria.IndexOf(x.Criterion)).Select(x => new AiInterviewScoreDto(x.Criterion, x.Score, x.Comment)).ToList();
        return new AiInterviewFeedbackDto(s.FeedbackSummary ?? string.Empty, SplitLines(s.Strengths), SplitLines(s.Improvements), scores,
            scores.Count == 0 ? 0 : Math.Round(scores.Average(x => x.Score), 1));
    }

    private static IReadOnlyList<string> SplitLines(string? text) =>
        string.IsNullOrWhiteSpace(text) ? [] : text.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
}

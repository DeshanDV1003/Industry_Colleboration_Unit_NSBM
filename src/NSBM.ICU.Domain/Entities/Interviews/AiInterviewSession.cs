using NSBM.ICU.Domain.Common;
using NSBM.ICU.Domain.Enums;
using NSBM.ICU.Domain.Rules;

namespace NSBM.ICU.Domain.Entities.Interviews;

/// <summary>
/// A practice interview with the AI interviewer. Private to the student: lecturers and companies never see it.
/// Turns alternate interviewer → student; the session ends after the last answer or when the student finishes early.
/// </summary>
public class AiInterviewSession : AuditableEntity
{
    public int StudentId { get; set; }
    public InterviewStage Stage { get; set; }
    public string TargetRole { get; set; } = string.Empty;
    /// <summary>Comma-separated skills the interviewer focuses on, fixed when the session starts.</summary>
    public string FocusSkills { get; set; } = string.Empty;
    public DateTime StartedAt { get; set; }
    /// <summary>When the student agreed that their answers are sent to the AI service.</summary>
    public DateTime ConsentedAt { get; set; }

    public AiInterviewStatus Status { get; private set; } = AiInterviewStatus.InProgress;
    public DateTime? CompletedAt { get; private set; }
    public string? FeedbackSummary { get; private set; }
    public string? Strengths { get; private set; }
    public string? Improvements { get; private set; }

    public ICollection<AiInterviewTurn> Turns { get; } = [];
    public ICollection<AiInterviewScore> Scores { get; } = [];

    public int QuestionCount => Turns.Count(t => t.Speaker == AiTurnSpeaker.Interviewer);
    public int AnswerCount => Turns.Count(t => t.Speaker == AiTurnSpeaker.Student);
    public bool AwaitingAnswer => Status == AiInterviewStatus.InProgress && LastSpeaker == AiTurnSpeaker.Interviewer;
    /// <summary>The last message is an answer and the interviewer has not replied yet (for example after a service error).</summary>
    public bool AwaitingInterviewer => Status == AiInterviewStatus.InProgress && LastSpeaker == AiTurnSpeaker.Student;
    private AiTurnSpeaker? LastSpeaker => Turns.OrderBy(t => t.Order).LastOrDefault()?.Speaker;
    public bool HasAskedAllQuestions => QuestionCount >= AiInterviewRubric.QuestionsPerSession;

    public void AskQuestion(string text, DateTime at)
    {
        EnsureInProgress();
        if (AwaitingAnswer) throw new DomainRuleException("The current question has not been answered yet.");
        if (HasAskedAllQuestions) throw new DomainRuleException("All questions for this session have been asked.");
        AddTurn(AiTurnSpeaker.Interviewer, text, at);
    }

    public void Answer(string text, DateTime at)
    {
        EnsureInProgress();
        if (!AwaitingAnswer) throw new DomainRuleException("There is no open question to answer.");
        AddTurn(AiTurnSpeaker.Student, text, at);
    }

    /// <summary>Records the AI's practice feedback. Needs at least one answer, and exactly one score per rubric criterion.</summary>
    public void Complete(string summary, IEnumerable<string> strengths, IEnumerable<string> improvements,
        IReadOnlyList<(string Criterion, int Score, string Comment)> scores, DateTime at)
    {
        EnsureInProgress();
        if (AnswerCount == 0) throw new DomainRuleException("Answer at least one question before finishing.");

        var criteria = AiInterviewRubric.CriteriaFor(Stage);
        if (scores.Count != criteria.Count || !criteria.All(c => scores.Any(s => s.Criterion == c)))
            throw new DomainRuleException("The feedback did not cover every rubric criterion.");

        foreach (var (criterion, score, comment) in scores)
        {
            Scores.Add(new AiInterviewScore
            {
                Criterion = criterion,
                Score = Math.Clamp(score, AiInterviewRubric.MinScore, AiInterviewRubric.MaxScore),
                Comment = comment,
            });
        }

        FeedbackSummary = summary;
        Strengths = string.Join('\n', strengths);
        Improvements = string.Join('\n', improvements);
        Status = AiInterviewStatus.Completed;
        CompletedAt = at;
    }

    private void AddTurn(AiTurnSpeaker speaker, string text, DateTime at) =>
        Turns.Add(new AiInterviewTurn { Order = Turns.Count + 1, Speaker = speaker, Text = text, CreatedAt = at });

    private void EnsureInProgress()
    {
        if (Status != AiInterviewStatus.InProgress) throw new DomainRuleException("This practice interview has already finished.");
    }
}

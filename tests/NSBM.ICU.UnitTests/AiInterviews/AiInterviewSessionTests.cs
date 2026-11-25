using NSBM.ICU.Domain.Common;
using NSBM.ICU.Domain.Entities.Interviews;
using NSBM.ICU.Domain.Enums;
using NSBM.ICU.Domain.Rules;

namespace NSBM.ICU.UnitTests.AiInterviews;

public sealed class AiInterviewSessionTests
{
    private static readonly DateTime Now = new(2026, 11, 22, 9, 0, 0, DateTimeKind.Utc);

    private static AiInterviewSession NewSession(InterviewStage stage = InterviewStage.SoftSkills) =>
        new() { StudentId = 1, Stage = stage, TargetRole = "Quality Assurance", StartedAt = Now, ConsentedAt = Now };

    private static List<(string, int, string)> FullScores(InterviewStage stage, int score = 4) =>
        AiInterviewRubric.CriteriaFor(stage).Select(c => (c, score, "ok")).ToList();

    [Fact]
    public void Questions_and_answers_alternate()
    {
        var s = NewSession();
        s.AskQuestion("Q1", Now);
        Assert.True(s.AwaitingAnswer);
        Assert.Throws<DomainRuleException>(() => s.AskQuestion("Q2", Now));

        s.Answer("A1", Now);
        Assert.True(s.AwaitingInterviewer);
        Assert.Throws<DomainRuleException>(() => s.Answer("A again", Now));
        Assert.Equal([1, 2], s.Turns.Select(t => t.Order));
    }

    [Fact]
    public void Cannot_ask_more_than_the_session_allows()
    {
        var s = NewSession();
        for (var i = 0; i < AiInterviewRubric.QuestionsPerSession; i++)
        {
            s.AskQuestion($"Q{i}", Now);
            s.Answer($"A{i}", Now);
        }
        Assert.True(s.HasAskedAllQuestions);
        Assert.Throws<DomainRuleException>(() => s.AskQuestion("one more", Now));
    }

    [Fact]
    public void Completing_needs_an_answer()
    {
        var s = NewSession();
        s.AskQuestion("Q1", Now);
        Assert.Throws<DomainRuleException>(() => s.Complete("x", [], [], FullScores(s.Stage), Now));
    }

    [Fact]
    public void Completing_needs_every_rubric_criterion_exactly()
    {
        var s = NewSession(InterviewStage.Technical);
        s.AskQuestion("Q1", Now);
        s.Answer("A1", Now);

        var missingOne = FullScores(s.Stage).Skip(1).ToList();
        Assert.Throws<DomainRuleException>(() => s.Complete("x", [], [], missingOne, Now));

        var wrongStage = FullScores(InterviewStage.SoftSkills);
        Assert.Throws<DomainRuleException>(() => s.Complete("x", [], [], wrongStage, Now));
        Assert.Equal(AiInterviewStatus.InProgress, s.Status);
    }

    [Fact]
    public void Completing_stores_feedback_clamps_scores_and_locks_the_session()
    {
        var s = NewSession();
        s.AskQuestion("Q1", Now);
        s.Answer("A1", Now);
        var scores = FullScores(s.Stage);
        scores[0] = (scores[0].Item1, 11, "too high");
        scores[1] = (scores[1].Item1, -2, "too low");

        s.Complete("Summary", ["Good"], ["Better"], scores, Now);

        Assert.Equal(AiInterviewStatus.Completed, s.Status);
        Assert.Equal(Now, s.CompletedAt);
        Assert.Equal(AiInterviewRubric.MaxScore, s.Scores.First(x => x.Criterion == scores[0].Item1).Score);
        Assert.Equal(AiInterviewRubric.MinScore, s.Scores.First(x => x.Criterion == scores[1].Item1).Score);
        Assert.False(s.AwaitingAnswer);
        Assert.Throws<DomainRuleException>(() => s.AskQuestion("Q2", Now));
    }
}

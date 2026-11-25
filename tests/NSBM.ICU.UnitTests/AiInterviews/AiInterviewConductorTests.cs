using NSBM.ICU.Application.Features.AiInterviews;
using NSBM.ICU.Domain.Entities.Interviews;
using NSBM.ICU.Domain.Enums;
using NSBM.ICU.Domain.Rules;

namespace NSBM.ICU.UnitTests.AiInterviews;

public sealed class AiInterviewConductorTests
{
    private static readonly DateTime Now = new(2026, 11, 22, 9, 0, 0, DateTimeKind.Utc);
    private readonly FakeAiInterviewer _ai = new();
    private readonly AiInterviewConductor _conductor;

    public AiInterviewConductorTests() => _conductor = new AiInterviewConductor(_ai, new FixedClock(Now));

    private static AiInterviewSession NewSession() => new()
    {
        StudentId = 1, Stage = InterviewStage.Technical, TargetRole = "Software Engineering",
        FocusSkills = "C#, SQL , Git", StartedAt = Now, ConsentedAt = Now,
    };

    [Fact]
    public async Task Opening_question_is_asked_with_the_session_brief()
    {
        var session = NewSession();
        await _conductor.AdvanceAsync(session, default);

        var (brief, transcript) = Assert.Single(_ai.QuestionCalls);
        Assert.Empty(transcript);
        Assert.Equal(1, brief.QuestionNumber);
        Assert.Equal(["C#", "SQL", "Git"], brief.FocusSkills);
        Assert.Equal(AiInterviewRubric.CriteriaFor(InterviewStage.Technical), brief.Criteria);
        Assert.Equal("Question 1?", session.Turns.Single().Text); // trimmed
    }

    [Fact]
    public async Task Full_interview_asks_every_question_then_completes_with_feedback()
    {
        var session = NewSession();
        await _conductor.AdvanceAsync(session, default);
        for (var i = 1; i <= AiInterviewRubric.QuestionsPerSession; i++)
        {
            session.Answer($"Answer {i}", Now);
            await _conductor.AdvanceAsync(session, default);
        }

        Assert.Equal(AiInterviewRubric.QuestionsPerSession, _ai.QuestionCalls.Count);
        Assert.Equal(AiInterviewRubric.QuestionsPerSession, _ai.QuestionCalls[^1].Brief.QuestionNumber);
        var transcript = Assert.Single(_ai.EvaluationCalls);
        Assert.Equal(AiInterviewRubric.QuestionsPerSession * 2, transcript.Count);
        Assert.Equal(AiTurnSpeaker.Student, transcript[^1].Speaker);

        Assert.Equal(AiInterviewStatus.Completed, session.Status);
        Assert.Equal("Clear examples", session.Strengths); // trimmed, blank items dropped
        Assert.Equal(AiInterviewRubric.MaxScore, session.Scores.Max(s => s.Score)); // 9 clamped to 5
    }

    [Fact]
    public async Task Finishing_early_evaluates_the_answers_so_far()
    {
        var session = NewSession();
        await _conductor.AdvanceAsync(session, default);
        session.Answer("Only answer", Now);
        await _conductor.AdvanceAsync(session, default); // asks question 2

        await _conductor.CompleteAsync(session, default);

        Assert.Equal(3, Assert.Single(_ai.EvaluationCalls).Count);
        Assert.Equal(AiInterviewStatus.Completed, session.Status);
    }
}

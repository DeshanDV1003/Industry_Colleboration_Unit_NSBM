using NSBM.ICU.Domain.Enums;

namespace NSBM.ICU.Domain.Rules;

/// <summary>
/// The fixed criteria the AI practice interviewer scores against, so feedback is comparable between sessions.
/// Scores run from 1 (needs work) to 5 (strong) and describe the answers given, never the person.
/// </summary>
public static class AiInterviewRubric
{
    public const int MinScore = 1;
    public const int MaxScore = 5;
    public const int QuestionsPerSession = 6;
    public const int MaxAnswerLength = 4000;

    private static readonly string[] SoftSkills =
    [
        "Structure (situation, action, result)",
        "Specific examples",
        "Clarity and conciseness",
        "Relevance to the role",
        "Reflection and learning",
    ];

    private static readonly string[] Technical =
    [
        "Technical accuracy",
        "Problem-solving approach",
        "Clarity of explanation",
        "Use of concrete examples",
        "Awareness of trade-offs",
    ];

    public static IReadOnlyList<string> CriteriaFor(InterviewStage stage) => stage == InterviewStage.Technical ? Technical : SoftSkills;
}

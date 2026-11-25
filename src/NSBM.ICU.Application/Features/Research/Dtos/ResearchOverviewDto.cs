using NSBM.ICU.Domain.Enums;

namespace NSBM.ICU.Application.Features.Research.Dtos;

public sealed record CountByLabelDto(string Label, int Count);

/// <summary>A core skill and how many students preparing for that path have neither listed nor practised it (RQ1).</summary>
public sealed record SkillGapDto(string SkillName, string CareerPathName, int StudentsMissing, int StudentsOnPath);

public sealed record ApplicationStatusCountDto(ApplicationStatus Status, int Count);

/// <summary>Sample counters for research planning. Demo data — these are not research findings.</summary>
public sealed record ResearchOverviewDto(
    int Students,
    int StudentsWithCareerPath,
    IReadOnlyList<CountByLabelDto> StudentsByCareerPath,
    int BaselineAttempts,
    int AverageBaselinePercent,
    int ApprovedTasks,
    int TaskAttempts,
    int TaskAttemptsAllChecksPassed,
    int KnowledgeChecks,
    int InterviewAnswers,
    int ApprovedVacancies,
    IReadOnlyList<ApplicationStatusCountDto> ApplicationsByStatus,
    int MentorQuestions,
    int MentorQuestionsAnswered,
    int SprintItemsDone,
    IReadOnlyList<SkillGapDto> TopSkillGaps);

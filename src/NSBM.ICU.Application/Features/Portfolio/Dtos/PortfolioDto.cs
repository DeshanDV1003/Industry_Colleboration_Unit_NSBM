using NSBM.ICU.Application.Features.Practice.Dtos;

namespace NSBM.ICU.Application.Features.Portfolio.Dtos;

public sealed record PortfolioTaskDto(
    int TaskId,
    string Title,
    string CompanyName,
    int Attempts,
    int BestPassedChecks,
    int TotalChecks,
    bool KnowledgeCheckDone,
    IReadOnlyList<AttemptReviewDto> Reviews);

public sealed record LecturerNoteDto(string LecturerName, string Text, DateTime CreatedAt);

public sealed record PortfolioInterviewDto(int SoftSkillsAnswered, int TechnicalAnswered, int AverageCoveragePercent);

/// <summary>
/// Task activity, practice evidence and feedback gathered in one place. Practice evidence shows growth;
/// it is not a certified measure of ability.
/// </summary>
public sealed record PortfolioDto(
    string FullName,
    string StudentNo,
    string Programme,
    string? CareerPathName,
    string? Headline,
    int RoadmapCompleted,
    int RoadmapTotal,
    int? BaselineScorePercent,
    IReadOnlyList<string> PractisedSkills,
    IReadOnlyList<string> ListedSkills,
    IReadOnlyList<PortfolioTaskDto> Tasks,
    PortfolioInterviewDto Interview,
    int SprintItemsDone,
    int ApplicationsSent,
    IReadOnlyList<LecturerNoteDto> LecturerNotes);

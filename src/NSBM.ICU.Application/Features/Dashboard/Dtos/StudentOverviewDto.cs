namespace NSBM.ICU.Application.Features.Dashboard.Dtos;

/// <summary>Headline learning status for the student dashboard.</summary>
public sealed record StudentOverviewDto(
    CareerChoiceDto? CareerPath,
    RoadmapProgressDto Roadmap,
    BaselineSummaryDto? Baseline);

public sealed record CareerChoiceDto(int Id, string Slug, string Name, string Tagline, DateTime? SelectedAt);

public sealed record RoadmapProgressDto(int Completed, int Total, int Percent, string? NextStepTitle);

public sealed record BaselineSummaryDto(DateTime CompletedAt, int ScorePercent, IReadOnlyList<string> StrongSkills, IReadOnlyList<string> FocusSkills);

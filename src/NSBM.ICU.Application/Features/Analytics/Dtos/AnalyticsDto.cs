namespace NSBM.ICU.Application.Features.Analytics.Dtos;

/// <summary>One Monday-to-Sunday week of activity, counted across all students.</summary>
public sealed record WeekActivityDto(DateOnly WeekStart, int ActiveStudents, int TaskAttempts, int InterviewAnswers, int Applications);

/// <summary>Applications to one company's vacancies, by where they stand.</summary>
public sealed record CompanyPipelineDto(string Company, int Submitted, int UnderReview, int Shortlisted, int InterviewInvited, int NotSelected)
{
    public int Total => Submitted + UnderReview + Shortlisted + InterviewInvited + NotSelected;
}

/// <summary><see cref="Attended"/> is null when too few registered to show it without describing individuals.</summary>
public sealed record EventAttendanceDto(string Event, DateOnly Date, int Registered, int? Attended);

/// <summary>Booked hours against teaching hours (07:00–21:00, Monday to Friday) in the coming weeks.</summary>
public sealed record VenueUsageDto(string Venue, int BookedHours, int AvailableHours);

/// <summary>
/// How far students on one career path have got. The baseline count and roadmap average are null for groups smaller
/// than <see cref="Domain.Rules.PrivacyRules.MinGroupSize"/>: over one or two people they would be personal results.
/// </summary>
public sealed record PathReadinessDto(string CareerPath, int Students, int? BaselineTaken, int? AverageRoadmapPercent);

/// <summary>
/// Aggregate figures for the Unit head and lecturers. Every number is a count or an average over a group;
/// nothing identifies, scores or ranks an individual student.
/// </summary>
public sealed record AnalyticsDto(
    int Weeks,
    int Students,
    int ActiveStudentsInPeriod,
    int ApplicationsInPeriod,
    int UpcomingHallWeeks,
    int MinGroupSize,
    IReadOnlyList<WeekActivityDto> Activity,
    IReadOnlyList<CompanyPipelineDto> Pipeline,
    IReadOnlyList<EventAttendanceDto> Events,
    IReadOnlyList<VenueUsageDto> Halls,
    IReadOnlyList<PathReadinessDto> Readiness);

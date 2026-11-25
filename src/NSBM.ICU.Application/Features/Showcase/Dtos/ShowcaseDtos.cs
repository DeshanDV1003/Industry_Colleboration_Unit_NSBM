using NSBM.ICU.Domain.Enums;

namespace NSBM.ICU.Application.Features.Showcase.Dtos;

/// <summary>Live platform counts for the public homepage. Aggregates only: nothing identifies a student.</summary>
public sealed record ShowcaseStatsDto(
    int PartnerCompanies,
    int OpenInternships,
    int IndustryTasks,
    int StudentsOnCareerPaths,
    int CertificatesIssued,
    int BadgesAwarded);

public sealed record ShowcaseCareerPathDto(string Slug, string Name, string Tagline, int SkillCount);

public sealed record ShowcasePartnerDto(string Name, int Tasks, int Internships, int Events);

public sealed record ShowcaseInternshipDto(string Title, string CompanyName, string CareerPathName, WorkMode WorkMode, string Location, DateOnly ClosingDate);

/// <param name="Kind">A Unit calendar type (e.g. "Programme") or a partner event format (e.g. "Hackathon").</param>
public sealed record ShowcaseEventDto(string Title, string Kind, string? Host, DateOnly StartDate, DateOnly EndDate, string? Location);

/// <summary>A student who chose to share their profile card. Only what the card itself shows.</summary>
public sealed record ShowcaseProfileDto(string FullName, string Programme, string? CareerPathName, IReadOnlyList<string> Badges, string CardSlug);

public sealed record ShowcaseDto(
    ShowcaseStatsDto Stats,
    IReadOnlyList<ShowcaseCareerPathDto> CareerPaths,
    IReadOnlyList<ShowcasePartnerDto> Partners,
    IReadOnlyList<ShowcaseInternshipDto> Internships,
    IReadOnlyList<ShowcaseEventDto> UpcomingEvents,
    IReadOnlyList<ShowcaseProfileDto> FeaturedProfiles);

public sealed record PartnerEnquiryDto(
    int Id,
    string OrganisationName,
    string ContactName,
    string Email,
    string? Phone,
    PartnerInterest Interest,
    string Message,
    DateTime SubmittedAt,
    bool IsHandled,
    DateTime? HandledAt,
    string? HandledBy);

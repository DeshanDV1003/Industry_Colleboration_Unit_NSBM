using NSBM.ICU.Domain.Enums;

namespace NSBM.ICU.Application.Features.Achievements.Dtos;

/// <param name="RecentWeeks">Whether each of the last weeks had practice, oldest first, ending this week.</param>
public sealed record ProgressDto(
    int CurrentStreak,
    int BestStreak,
    bool ActiveThisWeek,
    IReadOnlyList<bool> RecentWeeks,
    ProgressLevel Level,
    int RoadmapCompleted,
    int RoadmapTotal);

public sealed record BadgeDto(
    int Id,
    int SkillId,
    string SkillName,
    ModerationStatus Status,
    DateTime RequestedAt,
    DateTime? ReviewedAt,
    string? ReviewerName,
    string? ReviewNote,
    string? VerificationCode);

/// <summary>A skill the student has practice evidence for, with the badge status if they already asked.</summary>
public sealed record BadgeEligibleSkillDto(int SkillId, string SkillName, IReadOnlyList<string> Sources, ModerationStatus? BadgeStatus);

public sealed record ProfileCardSettingsDto(bool IsShared, string? Slug);

public sealed record AchievementsDto(
    ProgressDto Progress,
    IReadOnlyList<BadgeDto> Badges,
    IReadOnlyList<BadgeEligibleSkillDto> EligibleSkills,
    int CertificateCount,
    ProfileCardSettingsDto ProfileCard);

/// <summary>A badge request as a lecturer reviews it.</summary>
public sealed record BadgeReviewItemDto(
    int Id,
    string StudentName,
    string StudentNo,
    string Programme,
    string? CareerPathName,
    string SkillName,
    IReadOnlyList<string> Evidence,
    string? StudentNote,
    DateTime RequestedAt,
    ModerationStatus Status,
    string? ReviewerName,
    string? ReviewNote,
    DateTime? ReviewedAt);

public sealed record CardBadgeDto(string SkillName, string VerificationCode, DateTime ApprovedAt);

public sealed record CardCertificateDto(string EventTitle, string CompanyName, string Code, DateTime IssuedAt);

/// <summary>The public profile card. Holds only what the student chose to share: no grades, no scores, no contact details.</summary>
public sealed record ProfileCardDto(
    string FullName,
    string Programme,
    string? CareerPathName,
    ProgressLevel Level,
    int CurrentStreak,
    int BestStreak,
    IReadOnlyList<CardBadgeDto> Badges,
    IReadOnlyList<CardCertificateDto> Certificates);

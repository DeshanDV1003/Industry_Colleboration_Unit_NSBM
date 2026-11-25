using NSBM.ICU.Domain.Enums;

namespace NSBM.ICU.Application.Features.EventHub.Dtos;

/// <summary>
/// An approved event as a student sees it. <c>RegistrationOpen</c>: registrations and team changes are
/// still allowed. <c>IsOpen</c>: a new student can register now (open and not full).
/// </summary>
public sealed record EventCardDto(
    int Id,
    string Title,
    string CompanyName,
    EventFormat Format,
    DateOnly StartDate,
    DateOnly EndDate,
    string Location,
    int Capacity,
    int RegisteredCount,
    DateOnly RegistrationClosesOn,
    int? MinTeamSize,
    int? MaxTeamSize,
    bool IsRegistered,
    bool RegistrationOpen,
    bool IsOpen,
    bool HasEnded,
    bool HasCertificate);

public sealed record EventTeamSummaryDto(int Id, string Name, int MemberCount, bool IsFull);

/// <summary>The student's own team. The join code is shown only to members so they can invite others.</summary>
public sealed record MyTeamDto(int Id, string Name, string JoinCode, bool IsCaptain, IReadOnlyList<TeamMemberDto> Members);

public sealed record TeamMemberDto(string Name, bool IsCaptain, bool IsMe);

public sealed record EventDetailDto(
    EventCardDto Event,
    string Description,
    MyTeamDto? MyTeam,
    IReadOnlyList<EventTeamSummaryDto> Teams,
    string? MyCertificateCode);

/// <summary>A participation certificate. Anyone with the code can verify it.</summary>
public sealed record CertificateDto(
    string Code,
    string StudentName,
    string EventTitle,
    string CompanyName,
    EventFormat Format,
    DateOnly StartDate,
    DateOnly EndDate,
    string? TeamName,
    DateTime IssuedAt);

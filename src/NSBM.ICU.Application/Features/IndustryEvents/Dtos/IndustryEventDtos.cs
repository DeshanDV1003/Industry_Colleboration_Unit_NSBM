using NSBM.ICU.Domain.Enums;

namespace NSBM.ICU.Application.Features.IndustryEvents.Dtos;

/// <summary>A row on the partner's event list and the Unit's review board.</summary>
public sealed record IndustryEventListItemDto(
    int Id,
    string Title,
    string CompanyName,
    EventFormat Format,
    DateOnly StartDate,
    DateOnly EndDate,
    int Capacity,
    int RegisteredCount,
    bool IsTeamEvent,
    ModerationStatus Status,
    DateTime CreatedAt,
    DateTime? ReviewedAt,
    string? ReviewNote,
    bool CanBeEdited);

/// <summary>Full event for editing (partner) or review (Unit admin).</summary>
public sealed record IndustryEventDto(
    int Id,
    string Title,
    string Description,
    string CompanyName,
    EventFormat Format,
    DateOnly StartDate,
    DateOnly EndDate,
    string Location,
    int Capacity,
    DateOnly RegistrationClosesOn,
    int? MinTeamSize,
    int? MaxTeamSize,
    int RegisteredCount,
    ModerationStatus Status,
    string? ReviewedBy,
    string? ReviewNote,
    bool CanBeEdited,
    bool CanBeReviewed);

public sealed record EventAttendeeDto(
    int RegistrationId,
    string StudentName,
    string StudentNo,
    string Programme,
    string? TeamName,
    DateTime RegisteredAt,
    bool Attended,
    string? CertificateCode);

/// <summary>The host's attendance sheet: who registered, who came, who has a certificate.</summary>
public sealed record EventAttendanceDto(
    int EventId,
    string Title,
    DateOnly StartDate,
    DateOnly EndDate,
    bool HasStarted,
    bool HasEnded,
    IReadOnlyList<EventAttendeeDto> Attendees);

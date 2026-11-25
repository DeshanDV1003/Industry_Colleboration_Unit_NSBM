using NSBM.ICU.Domain.Enums;

namespace NSBM.ICU.Application.Features.Venues.Dtos;

public sealed record VenueDto(
    int Id,
    string Code,
    string Name,
    string Building,
    VenueType Type,
    int Capacity,
    string? Facilities,
    bool IsActive,
    int UpcomingBookings);

/// <summary>One allocation. Times are local campus times; <see cref="IsPast"/> bookings are kept as a record.</summary>
public sealed record VenueBookingDto(
    int Id,
    int VenueId,
    string VenueCode,
    string VenueName,
    VenueBookingPurpose Purpose,
    string Title,
    DateOnly Date,
    TimeOnly StartTime,
    TimeOnly EndTime,
    string? Programme,
    int? AcademicYear,
    int? IndustryEventId,
    int? CalendarEventId,
    int Attendees,
    string? Note,
    string BookedByName,
    bool IsPast);

/// <summary>A programme and academic year: the group a lecture hall is allocated to.</summary>
public sealed record CohortOptionDto(string Programme, int AcademicYear, int StudentCount);

public sealed record EventOptionDto(int Id, string Title, string? Host, DateOnly StartDate, DateOnly EndDate, int? Capacity, bool HasHall);

/// <summary>What the registrar can allocate a hall to.</summary>
public sealed record VenueBookingOptionsDto(
    IReadOnlyList<CohortOptionDto> Cohorts,
    IReadOnlyList<EventOptionDto> IndustryEvents,
    IReadOnlyList<EventOptionDto> UnitEvents);

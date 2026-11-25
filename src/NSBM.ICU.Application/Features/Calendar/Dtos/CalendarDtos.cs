using NSBM.ICU.Application.Features.Calendar.Models;
using NSBM.ICU.Domain.Enums;

namespace NSBM.ICU.Application.Features.Calendar.Dtos;

/// <summary>One entry on a viewer's calendar. Only Unit events carry an event id, type and audience.</summary>
public sealed record CalendarEntryDto(
    string Key,
    CalendarEntrySource Source,
    string Title,
    string? Description,
    DateOnly StartDate,
    DateOnly EndDate,
    int? EventId = null,
    CalendarEventType? EventType = null,
    CalendarAudience? Audience = null,
    string? Location = null,
    int? CareerPathId = null,
    string? CareerPathName = null,
    int? IndustryTaskId = null,
    int? VacancyId = null,
    int? IndustryEventId = null);

public sealed record CalendarOptionDto(int Id, string Name);

/// <summary>What the admin can link a calendar event to.</summary>
public sealed record CalendarOptionsDto(
    IReadOnlyList<CalendarOptionDto> CareerPaths,
    IReadOnlyList<CalendarOptionDto> Tasks,
    IReadOnlyList<CalendarOptionDto> Vacancies);

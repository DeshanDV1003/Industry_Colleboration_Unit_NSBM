using NSBM.ICU.Application.Common.Models;
using NSBM.ICU.Domain.Enums;

namespace NSBM.ICU.Application.Features.Vacancies.Dtos;

public sealed record VacancyListItemDto(
    int Id,
    string Title,
    string CompanyName,
    string CareerPathName,
    WorkMode WorkMode,
    string Location,
    DateOnly ClosingDate,
    ModerationStatus Status,
    DateTime CreatedAt,
    DateTime? ReviewedAt,
    string? ReviewNote,
    int ApplicationCount);

/// <summary>The complete vacancy, as seen by its company and by the Unit's reviewers.</summary>
public sealed record VacancyDto(
    int Id,
    string Title,
    string Description,
    int CareerPathId,
    string CareerPathName,
    WorkMode WorkMode,
    string Location,
    int DurationMonths,
    DateOnly ClosingDate,
    string CompanyName,
    ModerationStatus Status,
    DateTime CreatedAt,
    DateTime? ReviewedAt,
    string? ReviewedBy,
    string? ReviewNote,
    bool CanBeEdited,
    bool CanBeReviewed,
    IReadOnlyList<SkillRefDto> RequiredSkills,
    IReadOnlyList<SkillRefDto> PreferredSkills);

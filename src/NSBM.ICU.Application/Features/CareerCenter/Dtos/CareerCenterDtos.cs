using NSBM.ICU.Application.Common.Evidence;
using NSBM.ICU.Domain.Enums;

namespace NSBM.ICU.Application.Features.CareerCenter.Dtos;

/// <summary>An open vacancy compared with the student's profile. The match counts required-skill overlap only.</summary>
public sealed record VacancyMatchDto(
    int Id,
    string Title,
    string Description,
    string CompanyName,
    string CareerPathName,
    WorkMode WorkMode,
    string Location,
    int DurationMonths,
    DateOnly ClosingDate,
    int MatchPercent,
    IReadOnlyList<SkillEvidenceDto> Required,
    IReadOnlyList<SkillEvidenceDto> Preferred,
    ApplicationStatus? MyApplicationStatus);

public sealed record MyApplicationDto(
    int Id,
    int VacancyId,
    string VacancyTitle,
    string CompanyName,
    WorkMode WorkMode,
    ApplicationStatus Status,
    DateTime SubmittedAt,
    DateTime StatusChangedAt,
    int MatchPercentAtSubmission,
    string CoverNote);

using NSBM.ICU.Application.Common.Evidence;
using NSBM.ICU.Domain.Enums;

namespace NSBM.ICU.Application.Features.Candidates.Dtos;

public sealed record CompanyApplicationDto(
    int Id,
    int VacancyId,
    string VacancyTitle,
    int StudentId,
    string StudentName,
    string StudentNo,
    string Programme,
    ApplicationStatus Status,
    DateTime SubmittedAt,
    int MatchPercentAtSubmission,
    string CoverNote);

/// <summary>A student who opted in to profile sharing.</summary>
public sealed record SharedProfileDto(int StudentId, string StudentName, string StudentNo, string Programme, string? CareerPathName);

/// <summary>
/// Each vacancy requirement compared with the candidate's listed skills and practice signals. Supports a human
/// reviewer — it never makes a hiring decision.
/// </summary>
public sealed record CandidateEvidenceDto(
    int StudentId,
    string StudentName,
    string StudentNo,
    string Programme,
    string? CareerPathName,
    int VacancyId,
    string VacancyTitle,
    int MatchPercent,
    IReadOnlyList<SkillEvidenceDto> Required,
    IReadOnlyList<SkillEvidenceDto> Preferred,
    int? ApplicationId,
    ApplicationStatus? ApplicationStatus,
    string? CoverNote);

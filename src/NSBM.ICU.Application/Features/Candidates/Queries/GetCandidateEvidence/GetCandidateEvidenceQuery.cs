using MediatR;
using Microsoft.EntityFrameworkCore;
using NSBM.ICU.Application.Common.Evidence;
using NSBM.ICU.Application.Common.Exceptions;
using NSBM.ICU.Application.Common.Extensions;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Application.Features.Candidates.Dtos;

namespace NSBM.ICU.Application.Features.Candidates.Queries.GetCandidateEvidence;

/// <summary>
/// Compares a candidate with one of the company's vacancies. Allowed only when the student applied to that
/// vacancy or opted in to profile sharing.
/// </summary>
public sealed record GetCandidateEvidenceQuery(int StudentId, int VacancyId) : IRequest<CandidateEvidenceDto>;

public sealed class GetCandidateEvidenceQueryHandler(IApplicationDbContext db, ICurrentUser currentUser)
    : IRequestHandler<GetCandidateEvidenceQuery, CandidateEvidenceDto>
{
    public async Task<CandidateEvidenceDto> Handle(GetCandidateEvidenceQuery request, CancellationToken cancellationToken)
    {
        var partner = await db.GetCurrentPartnerAsync(currentUser, cancellationToken);

        var vacancy = await db.Vacancies.AsNoTracking()
            .Where(v => v.Id == request.VacancyId && v.CompanyId == partner.CompanyId)
            .Select(v => new
            {
                v.Title,
                Skills = v.Skills.OrderBy(s => s.Skill!.Name).Select(s => new { s.SkillId, s.Skill!.Name, s.IsRequired }).ToList(),
            })
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw NotFoundException.For("Vacancy", request.VacancyId);

        var student = await db.Students.AsNoTracking()
            .Where(s => s.Id == request.StudentId)
            .Select(s => new { s.Id, Name = s.FirstName + " " + s.LastName, s.StudentNo, s.Programme, s.ShareProfileWithPartners,
                Path = s.CareerPath != null ? s.CareerPath.Name : null })
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw NotFoundException.For("Student", request.StudentId);

        var application = await db.InternshipApplications.AsNoTracking()
            .Where(a => a.VacancyId == request.VacancyId && a.StudentId == request.StudentId)
            .Select(a => new { a.Id, a.Status, a.CoverNote })
            .FirstOrDefaultAsync(cancellationToken);

        if (application is null && !student.ShareProfileWithPartners)
            throw new ForbiddenAccessException("This student has not applied to the vacancy or shared their profile.");

        var evidence = await db.LoadStudentEvidenceAsync(student.Id, cancellationToken);
        var required = evidence.Describe(vacancy.Skills.Where(s => s.IsRequired).Select(s => (s.SkillId, s.Name)));
        var preferred = evidence.Describe(vacancy.Skills.Where(s => !s.IsRequired).Select(s => (s.SkillId, s.Name)));

        return new CandidateEvidenceDto(student.Id, student.Name, student.StudentNo, student.Programme, student.Path,
            request.VacancyId, vacancy.Title, StudentEvidence.MatchPercent(required), required, preferred,
            application?.Id, application?.Status, application?.CoverNote);
    }
}

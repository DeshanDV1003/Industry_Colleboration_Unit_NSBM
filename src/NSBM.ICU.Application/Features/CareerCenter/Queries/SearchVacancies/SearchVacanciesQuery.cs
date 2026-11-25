using MediatR;
using Microsoft.EntityFrameworkCore;
using NSBM.ICU.Application.Common.Evidence;
using NSBM.ICU.Application.Common.Extensions;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Application.Features.CareerCenter.Dtos;
using NSBM.ICU.Domain.Enums;

namespace NSBM.ICU.Application.Features.CareerCenter.Queries.SearchVacancies;

/// <summary>Open vacancies filtered by role, work mode and text, each with a rule-based match against the student's profile.</summary>
public sealed record SearchVacanciesQuery(int? CareerPathId, WorkMode? WorkMode, string? Search) : IRequest<IReadOnlyList<VacancyMatchDto>>;

public sealed class SearchVacanciesQueryHandler(IApplicationDbContext db, ICurrentUser currentUser, IDateTime clock)
    : IRequestHandler<SearchVacanciesQuery, IReadOnlyList<VacancyMatchDto>>
{
    public async Task<IReadOnlyList<VacancyMatchDto>> Handle(SearchVacanciesQuery request, CancellationToken cancellationToken)
    {
        var student = await db.GetCurrentStudentAsync(currentUser, cancellationToken);

        var vacancies = db.OpenVacancies(clock).AsNoTracking();
        if (request.CareerPathId is { } pathId) vacancies = vacancies.Where(v => v.CareerPathId == pathId);
        if (request.WorkMode is { } mode) vacancies = vacancies.Where(v => v.WorkMode == mode);
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var term = request.Search.Trim();
            vacancies = vacancies.Where(v => v.Title.Contains(term) || v.Company!.Name.Contains(term) || v.Location.Contains(term));
        }

        var rows = await vacancies
            .OrderBy(v => v.ClosingDate)
            .Select(v => new
            {
                v.Id, v.Title, v.Description, Company = v.Company!.Name, Path = v.CareerPath!.Name,
                v.WorkMode, v.Location, v.DurationMonths, v.ClosingDate,
                Skills = v.Skills.OrderBy(s => s.Skill!.Name).Select(s => new { s.SkillId, s.Skill!.Name, s.IsRequired }).ToList(),
                MyStatus = db.InternshipApplications
                    .Where(a => a.VacancyId == v.Id && a.StudentId == student.Id)
                    .Select(a => (ApplicationStatus?)a.Status)
                    .FirstOrDefault(),
            })
            .ToListAsync(cancellationToken);

        var evidence = await db.LoadStudentEvidenceAsync(student.Id, cancellationToken);

        return rows.Select(v =>
        {
            var required = evidence.Describe(v.Skills.Where(s => s.IsRequired).Select(s => (s.SkillId, s.Name)));
            var preferred = evidence.Describe(v.Skills.Where(s => !s.IsRequired).Select(s => (s.SkillId, s.Name)));
            return new VacancyMatchDto(v.Id, v.Title, v.Description, v.Company, v.Path, v.WorkMode, v.Location, v.DurationMonths,
                v.ClosingDate, StudentEvidence.MatchPercent(required), required, preferred, v.MyStatus);
        }).ToList();
    }
}

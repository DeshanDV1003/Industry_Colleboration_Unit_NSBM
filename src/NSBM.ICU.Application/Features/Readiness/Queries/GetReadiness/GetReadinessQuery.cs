using MediatR;
using Microsoft.EntityFrameworkCore;
using NSBM.ICU.Application.Common.Evidence;
using NSBM.ICU.Application.Common.Exceptions;
using NSBM.ICU.Application.Common.Extensions;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Application.Features.Readiness.Dtos;
using NSBM.ICU.Domain.Enums;

namespace NSBM.ICU.Application.Features.Readiness.Queries.GetReadiness;

/// <summary>Compares the student's skills with their career path, or with one approved vacancy when given.</summary>
public sealed record GetReadinessQuery(int? VacancyId) : IRequest<ReadinessDto>;

public sealed class GetReadinessQueryHandler(IApplicationDbContext db, ICurrentUser currentUser)
    : IRequestHandler<GetReadinessQuery, ReadinessDto>
{
    private sealed record Target(string Name, string? Company, List<(int Id, string Name, bool Required)> Skills);

    public async Task<ReadinessDto> Handle(GetReadinessQuery request, CancellationToken cancellationToken)
    {
        var student = await db.GetCurrentStudentAsync(currentUser, cancellationToken);

        var target = request.VacancyId is { } vacancyId
            ? await LoadVacancyAsync(vacancyId, cancellationToken)
            : student.CareerPathId is { } pathId ? await LoadCareerPathAsync(pathId, cancellationToken) : null;

        if (target is null)
            return new ReadinessDto(null, null, null, 0, [], []);

        var evidence = await db.LoadStudentEvidenceAsync(student.Id, cancellationToken);
        var required = evidence.Describe(target.Skills.Where(s => s.Required).Select(s => (s.Id, s.Name)));
        var preferred = evidence.Describe(target.Skills.Where(s => !s.Required).Select(s => (s.Id, s.Name)));

        return new ReadinessDto(target.Name, target.Company, request.VacancyId, StudentEvidence.MatchPercent(required), required, preferred);
    }

    private async Task<Target> LoadVacancyAsync(int vacancyId, CancellationToken ct)
    {
        var vacancy = await db.Vacancies.AsNoTracking()
            .Where(v => v.Id == vacancyId && v.Status == ModerationStatus.Approved)
            .Select(v => new
            {
                v.Title,
                Company = v.Company!.Name,
                Skills = v.Skills.OrderBy(s => s.Skill!.Name).Select(s => new { s.SkillId, s.Skill!.Name, s.IsRequired }).ToList(),
            })
            .FirstOrDefaultAsync(ct)
            ?? throw NotFoundException.For("Vacancy", vacancyId);

        return new Target(vacancy.Title, vacancy.Company, vacancy.Skills.Select(s => (s.SkillId, s.Name, s.IsRequired)).ToList());
    }

    private async Task<Target> LoadCareerPathAsync(int pathId, CancellationToken ct)
    {
        var path = await db.CareerPaths.AsNoTracking()
            .Where(p => p.Id == pathId)
            .Select(p => new
            {
                p.Name,
                Skills = p.Skills.OrderBy(s => s.Skill!.Name).Select(s => new { s.SkillId, s.Skill!.Name, s.Importance }).ToList(),
            })
            .SingleAsync(ct);

        return new Target(path.Name, null, path.Skills.Select(s => (s.SkillId, s.Name, s.Importance == SkillImportance.Core)).ToList());
    }
}

using Microsoft.EntityFrameworkCore;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Application.Common.Models;
using NSBM.ICU.Application.Features.Vacancies.Dtos;
using NSBM.ICU.Domain.Entities.Opportunities;

namespace NSBM.ICU.Application.Features.Vacancies;

/// <summary>Shared projections for the vacancy authoring and moderation handlers.</summary>
internal static class VacancyQueries
{
    public static Task<List<VacancyListItemDto>> ToListItemsAsync(this IQueryable<Vacancy> vacancies, IApplicationDbContext db, CancellationToken ct) =>
        vacancies.AsNoTracking()
            .Select(v => new VacancyListItemDto(
                v.Id,
                v.Title,
                v.Company!.Name,
                v.CareerPath!.Name,
                v.WorkMode,
                v.Location,
                v.ClosingDate,
                v.Status,
                v.CreatedAt,
                v.ReviewedAt,
                v.ReviewNote,
                db.InternshipApplications.Count(a => a.VacancyId == v.Id)))
            .ToListAsync(ct);

    /// <summary>Loads the vacancy graph and maps in memory so domain rules (CanBeEdited, CanBeReviewed) stay in one place.</summary>
    public static async Task<VacancyDto?> ToDetailAsync(this IQueryable<Vacancy> vacancies, CancellationToken ct)
    {
        var v = await vacancies.AsNoTracking()
            .Include(x => x.Company)
            .Include(x => x.CareerPath)
            .Include(x => x.Skills).ThenInclude(s => s.Skill)
            .FirstOrDefaultAsync(ct);

        if (v is null) return null;

        List<SkillRefDto> Skills(bool required) => v.Skills
            .Where(s => s.IsRequired == required)
            .OrderBy(s => s.Skill!.Name)
            .Select(s => new SkillRefDto(s.SkillId, s.Skill!.Name))
            .ToList();

        return new VacancyDto(v.Id, v.Title, v.Description, v.CareerPathId, v.CareerPath!.Name, v.WorkMode, v.Location,
            v.DurationMonths, v.ClosingDate, v.Company!.Name, v.Status, v.CreatedAt, v.ReviewedAt, v.ReviewedBy, v.ReviewNote,
            v.CanBeEdited, v.CanBeReviewed, Skills(required: true), Skills(required: false));
    }
}

using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Domain.Entities.Opportunities;
using NSBM.ICU.Domain.Enums;

namespace NSBM.ICU.Application.Features.CareerCenter;

internal static class OpenVacancyQueries
{
    /// <summary>Approved vacancies whose closing date has not passed — the only ones students can see or apply to.</summary>
    public static IQueryable<Vacancy> OpenVacancies(this IApplicationDbContext db, IDateTime clock)
    {
        var today = DateOnly.FromDateTime(clock.UtcNow);
        return db.Vacancies.Where(v => v.Status == ModerationStatus.Approved && v.ClosingDate >= today);
    }
}

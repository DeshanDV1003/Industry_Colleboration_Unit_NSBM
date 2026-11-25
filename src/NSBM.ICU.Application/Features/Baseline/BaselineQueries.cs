using Microsoft.EntityFrameworkCore;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Domain.Entities.Careers;
using NSBM.ICU.Domain.Entities.Learning;

namespace NSBM.ICU.Application.Features.Baseline;

/// <summary>Reusable loaders for the baseline feature's handlers.</summary>
internal static class BaselineQueries
{
    public static Task<List<BaselineQuestion>> LoadQuestionsAsync(this IApplicationDbContext db, int careerPathId, CancellationToken ct) =>
        db.BaselineQuestions.AsNoTracking()
            .Include(q => q.Skill)
            .Include(q => q.Options.OrderBy(o => o.SortOrder))
            .Where(q => q.CareerPathId == careerPathId)
            .OrderBy(q => q.SortOrder)
            .ToListAsync(ct);

    public static Task<BaselineAttempt?> LoadLatestAttemptAsync(this IApplicationDbContext db, int studentId, int careerPathId, CancellationToken ct) =>
        db.BaselineAttempts.AsNoTracking()
            .Include(a => a.Answers)
            .Where(a => a.StudentId == studentId && a.CareerPathId == careerPathId)
            .OrderByDescending(a => a.CompletedAt)
            .FirstOrDefaultAsync(ct);
}

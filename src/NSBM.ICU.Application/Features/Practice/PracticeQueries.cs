using Microsoft.EntityFrameworkCore;
using NSBM.ICU.Application.Common.Exceptions;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Application.Features.Practice.Dtos;
using NSBM.ICU.Domain.Entities.Learning;
using NSBM.ICU.Domain.Entities.Tasks;
using NSBM.ICU.Domain.Enums;

namespace NSBM.ICU.Application.Features.Practice;

/// <summary>Loaders and mapping shared by the practice handlers. Students only ever see approved tasks.</summary>
internal static class PracticeQueries
{
    public static IQueryable<IndustryTask> ApprovedTasks(this IApplicationDbContext db) =>
        db.IndustryTasks.Where(t => t.Status == ModerationStatus.Approved);

    public static async Task EnsureApprovedTaskExistsAsync(this IApplicationDbContext db, int taskId, CancellationToken ct)
    {
        if (!await db.ApprovedTasks().AnyAsync(t => t.Id == taskId, ct))
            throw NotFoundException.For("Task", taskId);
    }

    public static Task<List<TaskCheck>> LoadChecksAsync(this IApplicationDbContext db, int taskId, CancellationToken ct) =>
        db.IndustryTasks.AsNoTracking()
            .Where(t => t.Id == taskId)
            .SelectMany(t => t.Checks)
            .OrderBy(c => c.SortOrder)
            .ToListAsync(ct);

    public static TaskAttemptDto ToDto(this TaskAttempt attempt, IReadOnlyCollection<TaskCheck> checks)
    {
        var passedById = attempt.CheckResults.ToDictionary(r => r.CheckId, r => r.Passed);
        var results = checks
            .Where(c => passedById.ContainsKey(c.Id))
            .Select(c =>
            {
                var passed = passedById[c.Id];
                return new CheckResultDto(c.Id, c.Description, passed, passed ? null : c.Hint);
            })
            .ToList();

        return new TaskAttemptDto(attempt.Id, attempt.AttemptNumber, attempt.SubmittedAt,
            attempt.PassedChecks, attempt.TotalChecks, attempt.Code, results,
            attempt.Reviews.OrderBy(r => r.ReviewedAt).Select(r => new AttemptReviewDto(r.ReviewerName, r.Comment, r.Improvements, r.ReviewedAt)).ToList());
    }
}

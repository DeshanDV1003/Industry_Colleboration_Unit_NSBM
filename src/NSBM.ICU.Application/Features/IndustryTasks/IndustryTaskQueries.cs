using Microsoft.EntityFrameworkCore;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Application.Common.Models;
using NSBM.ICU.Application.Features.IndustryTasks.Dtos;
using NSBM.ICU.Domain.Entities.Tasks;

namespace NSBM.ICU.Application.Features.IndustryTasks;

/// <summary>Shared projections for the authoring and moderation handlers.</summary>
internal static class IndustryTaskQueries
{
    public static Task<List<IndustryTaskListItemDto>> ToListItemsAsync(this IQueryable<IndustryTask> tasks, IApplicationDbContext db, CancellationToken ct) =>
        tasks.AsNoTracking()
            .Select(t => new IndustryTaskListItemDto(
                t.Id,
                t.Title,
                t.Company!.Name,
                t.CareerPath!.Name,
                t.Level,
                t.Situation,
                t.Status,
                t.CreatedAt,
                t.ReviewedAt,
                t.ReviewNote,
                db.TaskAttempts.Count(a => a.TaskId == t.Id)))
            .ToListAsync(ct);

    /// <summary>Loads the full task graph. Mapping happens in memory so domain rules (CanBeEdited, CanBeReviewed) stay in one place.</summary>
    public static async Task<IndustryTaskDto?> ToDetailAsync(this IQueryable<IndustryTask> tasks, CancellationToken ct)
    {
        var task = await tasks.AsNoTracking()
            .Include(t => t.Company)
            .Include(t => t.CareerPath)
            .Include(t => t.Skills).ThenInclude(s => s.Skill)
            .Include(t => t.Checks)
            .Include(t => t.KnowledgeQuestions)
            .FirstOrDefaultAsync(ct);

        return task is null ? null : ToDto(task);
    }

    private static IndustryTaskDto ToDto(IndustryTask t) => new(
        t.Id,
        t.Title,
        t.Summary,
        t.Instructions,
        t.CareerPathId,
        t.CareerPath!.Name,
        t.Level,
        t.Situation,
        t.EstimatedMinutes,
        t.StarterLanguage,
        t.StarterCode,
        t.Company!.Name,
        t.Status,
        t.CreatedAt,
        t.ReviewedAt,
        t.ReviewedBy,
        t.ReviewNote,
        t.CanBeEdited,
        t.CanBeReviewed,
        t.Skills.OrderBy(s => s.Skill!.Name).Select(s => new SkillRefDto(s.SkillId, s.Skill!.Name)).ToList(),
        t.Checks.OrderBy(c => c.SortOrder).Select(c => new TaskCheckDto(c.Id, c.Description, c.Pattern, c.ShouldMatch, c.Hint)).ToList(),
        t.KnowledgeQuestions.OrderBy(q => q.SortOrder).Select(q => new KnowledgeQuestionDto(q.Id, q.Prompt)).ToList());
}

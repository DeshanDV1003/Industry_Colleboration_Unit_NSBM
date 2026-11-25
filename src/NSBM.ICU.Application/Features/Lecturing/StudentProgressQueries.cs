using Microsoft.EntityFrameworkCore;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Domain.Enums;
using NSBM.ICU.Domain.Rules;

namespace NSBM.ICU.Application.Features.Lecturing;

/// <summary>Headline progress facts for one student, used by the directory and the insights page.</summary>
internal sealed record StudentProgressSummary(
    DateTime? LastActivityAt,
    int TasksAttempted,
    int RoadmapCompleted,
    int RoadmapTotal,
    int? BaselinePercent,
    int TasksStruggling,
    IReadOnlyList<SupportSignal> Signals);

internal static class StudentProgressQueries
{
    public static async Task<Dictionary<int, StudentProgressSummary>> LoadProgressAsync(
        this IApplicationDbContext db, IReadOnlyCollection<int> studentIds, DateTime now, CancellationToken ct)
    {
        var students = await db.Students.AsNoTracking().Where(s => studentIds.Contains(s.Id))
            .Select(s => new { s.Id, s.CareerPathId }).ToListAsync(ct);

        var attempts = await db.TaskAttempts.AsNoTracking().Where(a => studentIds.Contains(a.StudentId))
            .GroupBy(a => new { a.StudentId, a.TaskId })
            .Select(g => new { g.Key.StudentId, Count = g.Count(), Best = g.Max(a => a.PassedChecks), Total = g.Max(a => a.TotalChecks), Last = g.Max(a => a.SubmittedAt) })
            .ToListAsync(ct);

        var baselines = (await db.BaselineAttempts.AsNoTracking().Where(a => studentIds.Contains(a.StudentId))
                .Select(a => new { a.StudentId, a.CareerPathId, a.CompletedAt, a.CorrectCount, a.QuestionCount }).ToListAsync(ct))
            .GroupBy(a => a.StudentId).ToDictionary(g => g.Key, g => g.ToList());

        var roadmapTotals = await db.RoadmapSteps.AsNoTracking().GroupBy(s => s.CareerPathId)
            .Select(g => new { PathId = g.Key, Count = g.Count() }).ToDictionaryAsync(x => x.PathId, x => x.Count, ct);
        var roadmapDone = await db.RoadmapStepCompletions.AsNoTracking().Where(c => studentIds.Contains(c.StudentId))
            .Select(c => new { c.StudentId, c.RoadmapStep!.CareerPathId, c.CompletedAt }).ToListAsync(ct);

        var otherActivity = await LoadOtherActivityAsync(db, studentIds, ct);
        var practisedOutsideTasks = await LoadPractisedOutsideTasksAsync(db, studentIds, ct);

        return students.ToDictionary(s => s.Id, s =>
        {
            var mine = attempts.Where(a => a.StudentId == s.Id).ToList();
            var studentBaselines = baselines.GetValueOrDefault(s.Id) ?? [];
            var latestBaseline = studentBaselines.Where(b => b.CareerPathId == s.CareerPathId).MaxBy(b => b.CompletedAt);
            var done = roadmapDone.Where(r => r.StudentId == s.Id).ToList();

            DateTime?[] candidates =
            [
                mine.Count == 0 ? null : mine.Max(a => a.Last),
                studentBaselines.Count == 0 ? null : studentBaselines.Max(b => b.CompletedAt),
                done.Count == 0 ? null : done.Max(r => r.CompletedAt),
                otherActivity.GetValueOrDefault(s.Id),
            ];
            var last = candidates.Where(d => d.HasValue).Max();

            var baselinePercent = latestBaseline is null ? (int?)null : SkillSignalRules.Percent(latestBaseline.CorrectCount, latestBaseline.QuestionCount);
            var struggling = mine.Count(a => SupportSignalRules.IsStruggling(a.Count, a.Best, a.Total));

            return new StudentProgressSummary(
                last,
                mine.Count,
                s.CareerPathId is null ? 0 : done.Count(r => r.CareerPathId == s.CareerPathId),
                s.CareerPathId is { } pathId ? roadmapTotals.GetValueOrDefault(pathId) : 0,
                baselinePercent,
                struggling,
                SupportSignalRules.Evaluate(s.CareerPathId is not null, mine.Count > 0 || practisedOutsideTasks.Contains(s.Id),
                    last, now, baselinePercent, struggling));
        });
    }

    /// <summary>Students with a baseline, knowledge check or interview answer (task attempts are counted separately).</summary>
    private static async Task<HashSet<int>> LoadPractisedOutsideTasksAsync(IApplicationDbContext db, IReadOnlyCollection<int> ids, CancellationToken ct)
    {
        var baseline = db.BaselineAttempts.Where(a => ids.Contains(a.StudentId)).Select(a => a.StudentId);
        var knowledge = db.KnowledgeCheckSubmissions.Where(k => ids.Contains(k.StudentId)).Select(k => k.StudentId);
        var interview = db.InterviewAnswers.Where(a => ids.Contains(a.StudentId)).Select(a => a.StudentId);
        return (await baseline.Union(knowledge).Union(interview).ToListAsync(ct)).ToHashSet();
    }

    /// <summary>Latest interview answer, knowledge check or application per student.</summary>
    private static async Task<Dictionary<int, DateTime?>> LoadOtherActivityAsync(IApplicationDbContext db, IReadOnlyCollection<int> ids, CancellationToken ct)
    {
        var interview = await db.InterviewAnswers.Where(a => ids.Contains(a.StudentId)).GroupBy(a => a.StudentId)
            .Select(g => new { Id = g.Key, Last = g.Max(a => a.AnsweredAt) }).ToListAsync(ct);
        var knowledge = await db.KnowledgeCheckSubmissions.Where(k => ids.Contains(k.StudentId)).GroupBy(k => k.StudentId)
            .Select(g => new { Id = g.Key, Last = g.Max(k => k.SubmittedAt) }).ToListAsync(ct);
        var applications = await db.InternshipApplications.Where(a => ids.Contains(a.StudentId)).GroupBy(a => a.StudentId)
            .Select(g => new { Id = g.Key, Last = g.Max(a => a.SubmittedAt) }).ToListAsync(ct);

        return interview.Concat(knowledge).Concat(applications)
            .GroupBy(x => x.Id)
            .ToDictionary(g => g.Key, g => (DateTime?)g.Max(x => x.Last));
    }
}

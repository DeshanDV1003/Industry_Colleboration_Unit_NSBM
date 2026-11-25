using Microsoft.EntityFrameworkCore;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Application.Features.Achievements.Dtos;
using NSBM.ICU.Domain.Entities.People;
using NSBM.ICU.Domain.Enums;
using NSBM.ICU.Domain.Rules;

namespace NSBM.ICU.Application.Features.Achievements;

/// <summary>Builds a student's streak and roadmap level. Shared by the achievements page and the public profile card.</summary>
internal static class ProgressQueries
{
    public const int RecentWeekCount = 12;

    public static async Task<ProgressDto> LoadProgressAsync(this IApplicationDbContext db, Student student, DateOnly today, CancellationToken ct)
    {
        var days = await db.LoadPracticeDaysAsync(student.Id, ct);

        var total = student.CareerPathId is { } pathId ? await db.RoadmapSteps.CountAsync(r => r.CareerPathId == pathId, ct) : 0;
        var completed = student.CareerPathId is { } path
            ? await db.RoadmapStepCompletions.CountAsync(c => c.StudentId == student.Id && c.RoadmapStep!.CareerPathId == path, ct)
            : 0;

        return new ProgressDto(
            StreakRules.CurrentStreak(days, today),
            StreakRules.BestStreak(days),
            days.Any(d => StreakRules.WeekStart(d) == StreakRules.WeekStart(today)),
            StreakRules.RecentWeeks(days, today, RecentWeekCount),
            ProgressLevelRules.LevelFor(completed, total),
            completed,
            total);
    }

    /// <summary>
    /// Every day the student practised: task attempts, knowledge checks, baseline, roadmap ticks, interview answers
    /// (standard and AI) and sprint work. Only the dates are read; nothing about the content.
    /// </summary>
    private static async Task<List<DateOnly>> LoadPracticeDaysAsync(this IApplicationDbContext db, int studentId, CancellationToken ct)
    {
        var moments = new List<DateTime>();
        moments.AddRange(await db.TaskAttempts.Where(a => a.StudentId == studentId).Select(a => a.SubmittedAt).ToListAsync(ct));
        moments.AddRange(await db.KnowledgeCheckSubmissions.Where(k => k.StudentId == studentId).Select(k => k.SubmittedAt).ToListAsync(ct));
        moments.AddRange(await db.BaselineAttempts.Where(b => b.StudentId == studentId).Select(b => b.CompletedAt).ToListAsync(ct));
        moments.AddRange(await db.RoadmapStepCompletions.Where(c => c.StudentId == studentId).Select(c => c.CompletedAt).ToListAsync(ct));
        moments.AddRange(await db.InterviewAnswers.Where(a => a.StudentId == studentId).Select(a => a.AnsweredAt).ToListAsync(ct));
        moments.AddRange(await db.SprintItemProgress.Where(p => p.StudentId == studentId).Select(p => p.UpdatedAt).ToListAsync(ct));
        moments.AddRange(await db.AiInterviewSessions.Where(s => s.StudentId == studentId)
            .SelectMany(s => s.Turns.Where(t => t.Speaker == AiTurnSpeaker.Student).Select(t => t.CreatedAt)).ToListAsync(ct));
        return moments.Select(DateOnly.FromDateTime).Distinct().ToList();
    }
}

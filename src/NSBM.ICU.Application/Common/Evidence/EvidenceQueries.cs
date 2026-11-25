using Microsoft.EntityFrameworkCore;
using NSBM.ICU.Application.Common.Interfaces;

namespace NSBM.ICU.Application.Common.Evidence;

public static class EvidenceQueries
{
    private const string BaselineSource = "Correct baseline answer";

    /// <summary>
    /// Builds a student's evidence. A practice signal comes from a correct baseline answer, an industry task
    /// attempt that passed every check, or a submitted knowledge check on a task that uses the skill.
    /// </summary>
    public static async Task<StudentEvidence> LoadStudentEvidenceAsync(this IApplicationDbContext db, int studentId, CancellationToken ct)
    {
        var listed = await db.StudentSkills
            .Where(s => s.StudentId == studentId)
            .Select(s => s.SkillId)
            .ToListAsync(ct);

        var baselineSkills = await db.BaselineAttempts
            .Where(a => a.StudentId == studentId)
            .SelectMany(a => a.Answers.Where(x => x.IsCorrect).Select(x => x.Question!.SkillId))
            .Distinct()
            .ToListAsync(ct);

        var passedTaskIds = db.TaskAttempts
            .Where(a => a.StudentId == studentId && a.TotalChecks > 0 && a.PassedChecks == a.TotalChecks)
            .Select(a => a.TaskId);
        var explainedTaskIds = db.KnowledgeCheckSubmissions
            .Where(k => k.StudentId == studentId)
            .Select(k => k.TaskId);

        var taskSkills = await db.IndustryTasks
            .Where(t => passedTaskIds.Contains(t.Id) || explainedTaskIds.Contains(t.Id))
            .SelectMany(t => t.Skills.Select(s => new { s.SkillId, t.Title }))
            .ToListAsync(ct);

        var sources = new Dictionary<int, List<string>>();
        void Add(int skillId, string source)
        {
            if (!sources.TryGetValue(skillId, out var list)) sources[skillId] = list = [];
            if (!list.Contains(source)) list.Add(source);
        }

        foreach (var skillId in baselineSkills) Add(skillId, BaselineSource);
        foreach (var item in taskSkills) Add(item.SkillId, $"Industry task: {item.Title}");

        return new StudentEvidence(
            listed.ToHashSet(),
            sources.ToDictionary(kv => kv.Key, kv => (IReadOnlyList<string>)kv.Value));
    }
}

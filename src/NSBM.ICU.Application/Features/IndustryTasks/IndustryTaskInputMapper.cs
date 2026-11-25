using Microsoft.EntityFrameworkCore;
using NSBM.ICU.Application.Common.Exceptions;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Application.Features.IndustryTasks.Models;
using NSBM.ICU.Domain.Entities.Tasks;

namespace NSBM.ICU.Application.Features.IndustryTasks;

/// <summary>Validates references in a task input and copies it onto an entity, replacing its child collections.</summary>
internal static class IndustryTaskInputMapper
{
    public static async Task EnsureReferencesExistAsync(this IApplicationDbContext db, IndustryTaskInput input, CancellationToken ct)
    {
        if (!await db.CareerPaths.AnyAsync(p => p.Id == input.CareerPathId, ct))
            throw new BusinessRuleException("The selected job role does not exist.");

        var known = await db.Skills.CountAsync(s => input.SkillIds.Contains(s.Id), ct);
        if (known != input.SkillIds.Count)
            throw new BusinessRuleException("One or more selected skills do not exist.");
    }

    public static void ApplyTo(this IndustryTaskInput input, IndustryTask task)
    {
        task.Title = input.Title.Trim();
        task.Summary = input.Summary.Trim();
        task.Instructions = input.Instructions.Trim();
        task.CareerPathId = input.CareerPathId;
        task.Level = input.Level;
        task.Situation = input.Situation;
        task.EstimatedMinutes = input.EstimatedMinutes;
        task.StarterLanguage = string.IsNullOrWhiteSpace(input.StarterLanguage) ? null : input.StarterLanguage.Trim();
        task.StarterCode = string.IsNullOrWhiteSpace(input.StarterCode) ? null : input.StarterCode;

        task.Skills.Clear();
        foreach (var skillId in input.SkillIds)
            task.Skills.Add(new TaskSkill { SkillId = skillId });

        task.Checks.Clear();
        var order = 0;
        foreach (var check in input.Checks)
        {
            task.Checks.Add(new TaskCheck
            {
                SortOrder = ++order,
                Description = check.Description.Trim(),
                Pattern = check.Pattern,
                ShouldMatch = check.ShouldMatch,
                Hint = string.IsNullOrWhiteSpace(check.Hint) ? null : check.Hint.Trim(),
            });
        }

        task.KnowledgeQuestions.Clear();
        order = 0;
        foreach (var prompt in input.KnowledgeQuestions)
            task.KnowledgeQuestions.Add(new KnowledgeQuestion { SortOrder = ++order, Prompt = prompt.Trim() });
    }
}

using Microsoft.EntityFrameworkCore;
using NSBM.ICU.Application.Common.Exceptions;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Application.Features.Vacancies.Models;
using NSBM.ICU.Domain.Entities.Opportunities;

namespace NSBM.ICU.Application.Features.Vacancies;

/// <summary>Validates references in a vacancy input and copies it onto an entity, replacing its skills.</summary>
internal static class VacancyInputMapper
{
    public static async Task EnsureReferencesExistAsync(this IApplicationDbContext db, VacancyInput input, CancellationToken ct)
    {
        if (!await db.CareerPaths.AnyAsync(p => p.Id == input.CareerPathId, ct))
            throw new BusinessRuleException("The selected job role does not exist.");

        var ids = input.RequiredSkillIds.Concat(input.PreferredSkillIds).ToList();
        if (await db.Skills.CountAsync(s => ids.Contains(s.Id), ct) != ids.Count)
            throw new BusinessRuleException("One or more selected skills do not exist.");
    }

    public static void ApplyTo(this VacancyInput input, Vacancy vacancy)
    {
        vacancy.Title = input.Title.Trim();
        vacancy.Description = input.Description.Trim();
        vacancy.CareerPathId = input.CareerPathId;
        vacancy.WorkMode = input.WorkMode;
        vacancy.Location = input.Location.Trim();
        vacancy.DurationMonths = input.DurationMonths;
        vacancy.ClosingDate = input.ClosingDate;

        vacancy.Skills.Clear();
        foreach (var id in input.RequiredSkillIds) vacancy.Skills.Add(new VacancySkill { SkillId = id, IsRequired = true });
        foreach (var id in input.PreferredSkillIds) vacancy.Skills.Add(new VacancySkill { SkillId = id, IsRequired = false });
    }
}

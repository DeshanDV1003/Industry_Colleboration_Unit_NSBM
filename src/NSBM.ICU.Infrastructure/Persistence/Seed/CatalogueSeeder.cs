using Microsoft.EntityFrameworkCore;
using NSBM.ICU.Domain.Entities.Careers;
using NSBM.ICU.Domain.Enums;
using NSBM.ICU.Infrastructure.Persistence.Seed.Data;

namespace NSBM.ICU.Infrastructure.Persistence.Seed;

/// <summary>Loads skills and the four career paths with their roadmaps and baseline questions.</summary>
public sealed class CatalogueSeeder(ApplicationDbContext db)
{
    public async Task SeedAsync(CancellationToken cancellationToken)
    {
        if (await db.CareerPaths.AnyAsync(cancellationToken))
            return;

        var skills = CareerCatalogueData.Skills
            .Select(s => new Skill { Name = s.Name, Category = s.Category, Description = s.Description })
            .ToDictionary(s => s.Name);
        db.Skills.AddRange(skills.Values);

        var order = 0;
        foreach (var seed in CareerCatalogueData.Paths)
            db.CareerPaths.Add(BuildPath(seed, ++order, skills));

        await db.SaveChangesAsync(cancellationToken);
    }

    private static CareerPath BuildPath(CareerCatalogueData.PathSeed seed, int sortOrder, IReadOnlyDictionary<string, Skill> skills)
    {
        var path = new CareerPath
        {
            Slug = seed.Slug,
            Name = seed.Name,
            Tagline = seed.Tagline,
            Description = seed.Description,
            EntryRoles = seed.EntryRoles,
            SortOrder = sortOrder,
        };

        foreach (var name in seed.CoreSkills)
            path.Skills.Add(new CareerPathSkill { Skill = skills[name], Importance = SkillImportance.Core });
        foreach (var name in seed.SupportingSkills)
            path.Skills.Add(new CareerPathSkill { Skill = skills[name], Importance = SkillImportance.Supporting });

        var step = 0;
        foreach (var s in seed.Roadmap)
        {
            path.RoadmapSteps.Add(new RoadmapStep
            {
                SortOrder = ++step,
                Stage = s.Stage,
                Title = s.Title,
                Description = s.Description,
                Outcome = s.Outcome,
                Skill = s.Skill is null ? null : skills[s.Skill],
            });
        }

        var question = 0;
        foreach (var q in seed.Baseline)
        {
            var entity = new BaselineQuestion
            {
                SortOrder = ++question,
                Skill = skills[q.Skill],
                Prompt = q.Prompt,
                Explanation = q.Explanation,
            };
            for (var i = 0; i < q.Options.Length; i++)
                entity.Options.Add(new BaselineOption { SortOrder = i + 1, Text = q.Options[i], IsCorrect = i == q.CorrectIndex });
            path.BaselineQuestions.Add(entity);
        }

        return path;
    }
}

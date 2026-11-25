using Microsoft.EntityFrameworkCore;
using NSBM.ICU.Domain.Entities.Tasks;
using NSBM.ICU.Infrastructure.Persistence.Seed.Data;

namespace NSBM.ICU.Infrastructure.Persistence.Seed;

/// <summary>Loads the fictional industry tasks: most approved, a few left pending for the admin review queue.</summary>
public sealed class TaskSeeder(ApplicationDbContext db)
{
    private const string Reviewer = "Pavithra Subhashini";

    public async Task SeedAsync(CancellationToken cancellationToken)
    {
        if (await db.IndustryTasks.AnyAsync(cancellationToken))
            return;

        var companies = await db.Companies.ToDictionaryAsync(c => c.Name, c => c.Id, cancellationToken);
        var partners = await db.IndustryPartners.ToDictionaryAsync(p => p.Email, p => p.Id, cancellationToken);
        var paths = await db.CareerPaths.ToDictionaryAsync(p => p.Slug, p => p.Id, cancellationToken);
        var skills = await db.Skills.ToDictionaryAsync(s => s.Name, s => s.Id, cancellationToken);
        var reviewedAt = DateTime.UtcNow;

        foreach (var seed in IndustryTaskData.Tasks)
        {
            var task = new IndustryTask
            {
                CompanyId = companies[seed.Company],
                AuthorPartnerId = partners[seed.AuthorEmail],
                CareerPathId = paths[seed.CareerPathSlug],
                Title = seed.Title,
                Summary = seed.Summary,
                Instructions = seed.Instructions,
                Level = seed.Level,
                Situation = seed.Situation,
                EstimatedMinutes = seed.EstimatedMinutes,
                StarterLanguage = seed.StarterLanguage,
                StarterCode = seed.StarterCode,
            };

            foreach (var skill in seed.Skills)
                task.Skills.Add(new TaskSkill { SkillId = skills[skill] });

            var order = 0;
            foreach (var check in seed.Checks)
                task.Checks.Add(new TaskCheck { SortOrder = ++order, Description = check.Description, Pattern = check.Pattern, ShouldMatch = check.ShouldMatch, Hint = check.Hint });

            order = 0;
            foreach (var prompt in seed.KnowledgeQuestions)
                task.KnowledgeQuestions.Add(new KnowledgeQuestion { SortOrder = ++order, Prompt = prompt });

            if (seed.Approved)
                task.Approve(Reviewer, reviewedAt, note: null);

            db.IndustryTasks.Add(task);
        }

        await db.SaveChangesAsync(cancellationToken);
    }
}

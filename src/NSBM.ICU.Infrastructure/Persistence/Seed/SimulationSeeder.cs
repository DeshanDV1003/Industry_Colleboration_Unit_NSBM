using Microsoft.EntityFrameworkCore;
using NSBM.ICU.Domain.Entities.Mentoring;
using NSBM.ICU.Domain.Entities.Simulation;
using NSBM.ICU.Infrastructure.Persistence.Seed.Data;

namespace NSBM.ICU.Infrastructure.Persistence.Seed;

/// <summary>Loads the practice sprint board and a sample answered mentor question for the demo cohort.</summary>
public sealed class SimulationSeeder(ApplicationDbContext db)
{
    public async Task SeedAsync(CancellationToken cancellationToken)
    {
        if (await db.SprintWorkItems.AnyAsync(cancellationToken))
            return;

        var paths = await db.CareerPaths.ToDictionaryAsync(p => p.Slug, p => p.Id, cancellationToken);
        var order = 0;
        foreach (var seed in SimulationData.WorkItems)
        {
            db.SprintWorkItems.Add(new SprintWorkItem
            {
                Key = seed.Key,
                CareerPathId = paths[seed.CareerPathSlug],
                Title = seed.Title,
                Description = seed.Description,
                StoryPoints = seed.Points,
                TeammateName = seed.Teammate,
                TeammateStatus = seed.TeammateStatus,
                SortOrder = ++order,
            });
        }

        var kavindi = await db.Students.FirstOrDefaultAsync(s => s.StudentNo == "DEMO-002", cancellationToken);
        if (kavindi is not null)
        {
            var now = DateTime.UtcNow;
            var question = new MentorQuestion
            {
                StudentId = kavindi.Id,
                CareerPathId = kavindi.CareerPathId,
                Title = "How much SQL should an intern know?",
                Body = "I can write joins and GROUP BY queries. Is that enough for a software engineering internship, or should I learn more?",
                AskedAt = now.AddDays(-2),
            };
            question.Answer(
                "Joins, GROUP BY and filtering are a great base. Also practise indexes and reading a query plan at a basic level — and be ready to explain a query you wrote.",
                "Dilan Rajapaksha", PeopleSeeder.SerendibSoftwareLabs, now.AddDays(-1));
            db.MentorQuestions.Add(question);
        }

        await db.SaveChangesAsync(cancellationToken);
    }
}

using Microsoft.EntityFrameworkCore;
using NSBM.ICU.Domain.Entities.Interviews;
using NSBM.ICU.Domain.Enums;
using NSBM.ICU.Infrastructure.Persistence.Seed.Data;

namespace NSBM.ICU.Infrastructure.Persistence.Seed;

/// <summary>Loads the soft-skills and technical practice interview questions.</summary>
public sealed class InterviewSeeder(ApplicationDbContext db)
{
    public async Task SeedAsync(CancellationToken cancellationToken)
    {
        if (await db.InterviewQuestions.AnyAsync(cancellationToken))
            return;

        var order = 0;
        foreach (var seed in InterviewData.SoftSkills)
            db.InterviewQuestions.Add(Build(seed, InterviewStage.SoftSkills, careerPathId: null, ++order));

        var paths = await db.CareerPaths.ToDictionaryAsync(p => p.Slug, p => p.Id, cancellationToken);
        foreach (var (slug, seeds) in InterviewData.Technical)
        {
            order = 0;
            foreach (var seed in seeds)
                db.InterviewQuestions.Add(Build(seed, InterviewStage.Technical, paths[slug], ++order));
        }

        await db.SaveChangesAsync(cancellationToken);
    }

    private static InterviewQuestion Build(InterviewData.QuestionSeed seed, InterviewStage stage, int? careerPathId, int sortOrder)
    {
        var question = new InterviewQuestion
        {
            Stage = stage,
            CareerPathId = careerPathId,
            SortOrder = sortOrder,
            Topic = seed.Topic,
            Prompt = seed.Prompt,
            Keywords = string.Join(';', seed.Keywords),
            Tip = seed.Tip,
        };

        var followUpOrder = 0;
        foreach (var followUp in seed.FollowUps ?? [])
            question.FollowUps.Add(Build(followUp, stage, careerPathId, ++followUpOrder));

        return question;
    }
}

using Microsoft.EntityFrameworkCore;
using NSBM.ICU.Domain.Entities.Learning;
using NSBM.ICU.Domain.Entities.People;
using NSBM.ICU.Domain.Entities.Profiles;
using NSBM.ICU.Domain.Rules;

namespace NSBM.ICU.Infrastructure.Persistence.Seed;

/// <summary>
/// Sample badges backed by real evidence: each student first gets a baseline attempt in which they answered the
/// skill's question correctly, exactly what the evidence engine counts. Kavindi's SQL badge is approved and her
/// profile card is shared; Amaya's networking request waits for a lecturer. Runs after the opportunity seeder,
/// which gives the cohort their career paths.
/// </summary>
public sealed class AchievementSeeder(ApplicationDbContext db)
{
    private const string BaselineEvidence = "Correct baseline answer";

    public async Task SeedAsync(CancellationToken cancellationToken)
    {
        if (await db.SkillBadges.AnyAsync(cancellationToken))
            return;

        var now = DateTime.UtcNow;
        var students = await db.Students.ToDictionaryAsync(s => s.StudentNo, cancellationToken);
        var reviewer = await db.Lecturers.OrderBy(l => l.Id).Select(l => l.FirstName + " " + l.LastName).FirstOrDefaultAsync(cancellationToken)
                       ?? "Faculty of Computing";

        if (students.TryGetValue("DEMO-002", out var kavindi)
            && await AddBaselineEvidenceAsync(kavindi, "SQL", now.AddDays(-8), cancellationToken) is { } sql)
        {
            var badge = SkillBadge.Request(kavindi.Id, sql, BaselineEvidence, "I also used joins and GROUP BY in my second-year database project.", now.AddDays(-6));
            badge.Approve(reviewer, ShareCodeRules.NewBadgeCode(), "The baseline answer and your project note support it.", now.AddDays(-5));
            db.SkillBadges.Add(badge);
            kavindi.SetProfileCardSharing(true, ShareCodeRules.NewProfileSlug);
        }

        if (students.TryGetValue("DEMO-004", out var amaya)
            && await AddBaselineEvidenceAsync(amaya, "Networking basics", now.AddDays(-2), cancellationToken) is { } networking)
        {
            db.SkillBadges.Add(SkillBadge.Request(amaya.Id, networking, BaselineEvidence,
                "I configured VLANs and subnets in the networking lab.", now.AddDays(-1)));
        }

        await db.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Records a baseline attempt on the student's path: the given skill's questions answered correctly, the rest
    /// alternating. Returns the skill id, or null when the path has no question for that skill.
    /// </summary>
    private async Task<int?> AddBaselineEvidenceAsync(Student student, string skillName, DateTime at, CancellationToken ct)
    {
        if (student.CareerPathId is not { } pathId) return null;
        var questions = await db.BaselineQuestions.Include(q => q.Skill).Include(q => q.Options)
            .Where(q => q.CareerPathId == pathId).OrderBy(q => q.SortOrder).ToListAsync(ct);
        var skillId = questions.FirstOrDefault(q => q.Skill!.Name == skillName)?.SkillId;
        if (skillId is null) return null;

        var attempt = new BaselineAttempt { StudentId = student.Id, CareerPathId = pathId, CompletedAt = at, QuestionCount = questions.Count };
        for (var i = 0; i < questions.Count; i++)
        {
            var question = questions[i];
            var answerCorrectly = question.SkillId == skillId || i % 2 == 0;
            var option = question.Options.OrderBy(o => o.SortOrder).First(o => o.IsCorrect == answerCorrectly);
            attempt.Answers.Add(new BaselineAnswer { QuestionId = question.Id, SelectedOptionId = option.Id, IsCorrect = option.IsCorrect });
        }
        attempt.CorrectCount = attempt.Answers.Count(a => a.IsCorrect);
        db.BaselineAttempts.Add(attempt);
        return skillId;
    }
}

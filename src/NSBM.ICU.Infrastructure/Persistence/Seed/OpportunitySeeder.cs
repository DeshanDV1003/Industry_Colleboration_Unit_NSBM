using Microsoft.EntityFrameworkCore;
using NSBM.ICU.Domain.Entities.Opportunities;
using NSBM.ICU.Domain.Entities.Profiles;
using NSBM.ICU.Domain.Enums;
using NSBM.ICU.Domain.Rules;
using NSBM.ICU.Infrastructure.Persistence.Seed.Data;

namespace NSBM.ICU.Infrastructure.Persistence.Seed;

/// <summary>Loads fictional vacancies and gives the demo cohort career paths, skills and applications.</summary>
public sealed class OpportunitySeeder(ApplicationDbContext db)
{
    private const string Reviewer = "Pavithra Subhashini";

    public async Task SeedAsync(CancellationToken cancellationToken)
    {
        if (await db.Vacancies.AnyAsync(cancellationToken))
            return;

        var now = DateTime.UtcNow;
        var today = DateOnly.FromDateTime(now);
        var companies = await db.Companies.ToDictionaryAsync(c => c.Name, c => c.Id, cancellationToken);
        var partners = await db.IndustryPartners.ToDictionaryAsync(p => p.Email, p => p.Id, cancellationToken);
        var paths = await db.CareerPaths.ToDictionaryAsync(p => p.Slug, p => p.Id, cancellationToken);
        var skills = await db.Skills.ToDictionaryAsync(s => s.Name, s => s.Id, cancellationToken);

        var vacancies = new Dictionary<string, Vacancy>();
        foreach (var seed in OpportunityData.Vacancies)
        {
            var vacancy = new Vacancy
            {
                CompanyId = companies[seed.Company],
                AuthorPartnerId = partners[seed.AuthorEmail],
                CareerPathId = paths[seed.CareerPathSlug],
                Title = seed.Title,
                Description = seed.Description,
                WorkMode = seed.WorkMode,
                Location = seed.Location,
                DurationMonths = seed.DurationMonths,
                ClosingDate = today.AddDays(seed.ClosesInDays),
            };
            foreach (var name in seed.Required) vacancy.Skills.Add(new VacancySkill { SkillId = skills[name], IsRequired = true });
            foreach (var name in seed.Preferred) vacancy.Skills.Add(new VacancySkill { SkillId = skills[name], IsRequired = false });
            if (seed.Approved) vacancy.Approve(Reviewer, now, note: null);

            db.Vacancies.Add(vacancy);
            vacancies[seed.Key] = vacancy;
        }
        await db.SaveChangesAsync(cancellationToken);

        var students = await db.Students.ToDictionaryAsync(s => s.StudentNo, cancellationToken);
        foreach (var cohort in OpportunityData.Cohort)
        {
            if (!students.TryGetValue(cohort.StudentNo, out var student)) continue;

            if (student.CareerPathId is null) student.ChooseCareerPath(paths[cohort.CareerPathSlug], now);
            student.ShareProfileWithPartners = cohort.SharesProfile;

            var listed = cohort.Skills.Select(name => skills[name]).ToHashSet();
            foreach (var skillId in listed)
                db.StudentSkills.Add(new StudentSkill { StudentId = student.Id, SkillId = skillId, AddedAt = now });

            if (cohort.Application is { } seed)
            {
                var vacancy = vacancies[seed.VacancyKey];
                var required = vacancy.Skills.Where(s => s.IsRequired)
                    .Select(s => SkillMatchRules.Classify(listed.Contains(s.SkillId), practised: false))
                    .ToList();

                var application = new InternshipApplication
                {
                    VacancyId = vacancy.Id,
                    StudentId = student.Id,
                    CoverNote = seed.CoverNote,
                    MatchPercentAtSubmission = SkillMatchRules.MatchPercent(required),
                };
                application.Submit(now.AddDays(-3));
                if (seed.Status != ApplicationStatus.Submitted) application.ChangeStatus(seed.Status, now.AddDays(-1));
                db.InternshipApplications.Add(application);
            }
        }

        await db.SaveChangesAsync(cancellationToken);
    }
}

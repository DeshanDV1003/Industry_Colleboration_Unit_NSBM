using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Infrastructure.Identity;
using NSBM.ICU.Infrastructure.Persistence;
using NSBM.ICU.Infrastructure.Persistence.Seed;

namespace NSBM.ICU.Infrastructure.Services;

/// <summary>
/// Clears every activity and partner-content table (children before parents), resets student choices and
/// re-runs the content seeders. Companies and the catalogue are kept; seeded accounts are reactivated with their demo passwords, other accounts removed, and venues return to the seeded list.
/// </summary>
public sealed class DemoDataService(
    ApplicationDbContext db,
    IOptions<DemoOptions> options,
    TaskSeeder tasks,
    OpportunitySeeder opportunities,
    SimulationSeeder simulation,
    EventSeeder events,
    AchievementSeeder achievements,
    CalendarSeeder calendar,
    VenueSeeder venues,
    PeopleSeeder people) : IDemoDataService
{
    public bool IsEnabled => options.Value.Enabled;

    public async Task ResetAsync(CancellationToken ct)
    {
        await using var transaction = await db.Database.BeginTransactionAsync(ct);

        await db.Notifications.ExecuteDeleteAsync(ct);
        await db.Announcements.ExecuteDeleteAsync(ct);
        await db.EmailPreferences.ExecuteDeleteAsync(ct);
        await db.VenueBookings.ExecuteDeleteAsync(ct);
        await db.Venues.ExecuteDeleteAsync(ct);
        await db.PartnerEnquiries.ExecuteDeleteAsync(ct);
        await db.CalendarEvents.ExecuteDeleteAsync(ct);
        await db.TaskAssignments.ExecuteDeleteAsync(ct);
        await db.SupportNotes.ExecuteDeleteAsync(ct);
        await db.TaskAttemptReviews.ExecuteDeleteAsync(ct);
        await db.TaskAttempts.ExecuteDeleteAsync(ct);
        await db.KnowledgeCheckSubmissions.ExecuteDeleteAsync(ct);
        await db.InterviewAnswers.ExecuteDeleteAsync(ct);
        await db.AiInterviewSessions.ExecuteDeleteAsync(ct);
        await db.BaselineAttempts.ExecuteDeleteAsync(ct);
        await db.RoadmapStepCompletions.ExecuteDeleteAsync(ct);
        await db.PlannerItems.ExecuteDeleteAsync(ct);
        await db.CvProfiles.ExecuteDeleteAsync(ct);
        await db.StudentSkills.ExecuteDeleteAsync(ct);
        await db.SkillBadges.ExecuteDeleteAsync(ct);
        await db.MentorQuestions.ExecuteDeleteAsync(ct);
        await db.SprintItemProgress.ExecuteDeleteAsync(ct);
        await db.SprintWorkItems.ExecuteDeleteAsync(ct);
        await db.EventRegistrations.ExecuteDeleteAsync(ct);
        await db.IndustryEvents.ExecuteDeleteAsync(ct);
        await db.InternshipApplications.ExecuteDeleteAsync(ct);
        await db.Vacancies.ExecuteDeleteAsync(ct);
        await db.IndustryTasks.ExecuteDeleteAsync(ct);
        // Accounts the admin or the tests created go too (their activity was cleared above); seeded people are reactivated.
        var seeded = PeopleSeeder.SeededEmails.Select(e => e.ToUpperInvariant()).ToList();
        await db.Users.Where(u => !seeded.Contains(u.NormalizedEmail!)).ExecuteDeleteAsync(ct);
        await db.Users.ExecuteUpdateAsync(u => u.SetProperty(x => x.IsActive, true), ct);

        await db.Students.ExecuteUpdateAsync(s => s
            .SetProperty(x => x.CareerPathId, (int?)null)
            .SetProperty(x => x.CareerPathSelectedAt, (DateTime?)null)
            .SetProperty(x => x.ShareProfileWithPartners, false)
            .SetProperty(x => x.ShareProfileCard, false)
            .SetProperty(x => x.ProfileCardSlug, (string?)null), ct);

        db.ChangeTracker.Clear();
        await tasks.SeedAsync(ct);
        await opportunities.SeedAsync(ct);
        await simulation.SeedAsync(ct);
        await events.SeedAsync(ct);
        await achievements.SeedAsync(ct);
        await calendar.SeedAsync(ct);
        await venues.SeedAsync(ct);
        await db.MarkSeededNotificationsEmailedAsync(ct);

        await transaction.CommitAsync(ct);
        await people.RestorePasswordsAsync();
    }
}

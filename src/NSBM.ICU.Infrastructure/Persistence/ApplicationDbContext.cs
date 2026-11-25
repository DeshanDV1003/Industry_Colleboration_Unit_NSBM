using System.Reflection;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Domain.Entities.Auditing;
using NSBM.ICU.Domain.Entities.Calendar;
using NSBM.ICU.Domain.Entities.Careers;
using NSBM.ICU.Domain.Entities.Events;
using NSBM.ICU.Domain.Entities.Interviews;
using NSBM.ICU.Domain.Entities.Learning;
using NSBM.ICU.Domain.Entities.Mentoring;
using NSBM.ICU.Domain.Entities.Notifications;
using NSBM.ICU.Domain.Entities.Opportunities;
using NSBM.ICU.Domain.Entities.Organisations;
using NSBM.ICU.Domain.Entities.People;
using NSBM.ICU.Domain.Entities.Planning;
using NSBM.ICU.Domain.Entities.Profiles;
using NSBM.ICU.Domain.Entities.Simulation;
using NSBM.ICU.Domain.Entities.Support;
using NSBM.ICU.Domain.Entities.Tasks;
using NSBM.ICU.Domain.Entities.Venues;
using NSBM.ICU.Infrastructure.Identity;

namespace NSBM.ICU.Infrastructure.Persistence;

public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<ApplicationUser>(options), IApplicationDbContext
{
    public DbSet<Company> Companies => Set<Company>();
    public DbSet<PartnerEnquiry> PartnerEnquiries => Set<PartnerEnquiry>();

    public DbSet<Student> Students => Set<Student>();
    public DbSet<Lecturer> Lecturers => Set<Lecturer>();
    public DbSet<IndustryPartner> IndustryPartners => Set<IndustryPartner>();
    public DbSet<Administrator> Administrators => Set<Administrator>();
    public DbSet<Registrar> Registrars => Set<Registrar>();
    public DbSet<StudentSkill> StudentSkills => Set<StudentSkill>();
    public DbSet<CvProfile> CvProfiles => Set<CvProfile>();
    public DbSet<PlannerItem> PlannerItems => Set<PlannerItem>();

    public DbSet<Skill> Skills => Set<Skill>();
    public DbSet<CareerPath> CareerPaths => Set<CareerPath>();
    public DbSet<CareerPathSkill> CareerPathSkills => Set<CareerPathSkill>();
    public DbSet<RoadmapStep> RoadmapSteps => Set<RoadmapStep>();
    public DbSet<BaselineQuestion> BaselineQuestions => Set<BaselineQuestion>();

    public DbSet<BaselineAttempt> BaselineAttempts => Set<BaselineAttempt>();
    public DbSet<RoadmapStepCompletion> RoadmapStepCompletions => Set<RoadmapStepCompletion>();
    public DbSet<TaskAttempt> TaskAttempts => Set<TaskAttempt>();
    public DbSet<KnowledgeCheckSubmission> KnowledgeCheckSubmissions => Set<KnowledgeCheckSubmission>();
    public DbSet<InterviewAnswer> InterviewAnswers => Set<InterviewAnswer>();
    public DbSet<TaskAttemptReview> TaskAttemptReviews => Set<TaskAttemptReview>();
    public DbSet<SprintItemProgress> SprintItemProgress => Set<SprintItemProgress>();

    public DbSet<MentorQuestion> MentorQuestions => Set<MentorQuestion>();
    public DbSet<SprintWorkItem> SprintWorkItems => Set<SprintWorkItem>();

    public DbSet<InterviewQuestion> InterviewQuestions => Set<InterviewQuestion>();

    public DbSet<SupportNote> SupportNotes => Set<SupportNote>();
    public DbSet<TaskAssignment> TaskAssignments => Set<TaskAssignment>();

    public DbSet<IndustryTask> IndustryTasks => Set<IndustryTask>();

    public DbSet<Vacancy> Vacancies => Set<Vacancy>();
    public DbSet<InternshipApplication> InternshipApplications => Set<InternshipApplication>();

    public DbSet<CalendarEvent> CalendarEvents => Set<CalendarEvent>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<Announcement> Announcements => Set<Announcement>();
    public DbSet<AuditEntry> AuditEntries => Set<AuditEntry>();
    public DbSet<Venue> Venues => Set<Venue>();
    public DbSet<VenueBooking> VenueBookings => Set<VenueBooking>();
    public DbSet<AiInterviewSession> AiInterviewSessions => Set<AiInterviewSession>();
    public DbSet<SkillBadge> SkillBadges => Set<SkillBadge>();

    public DbSet<IndustryEvent> IndustryEvents => Set<IndustryEvent>();
    public DbSet<EventRegistration> EventRegistrations => Set<EventRegistration>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        // SQL Server does not keep DateTimeKind. Every timestamp is written as UTC, so mark it UTC on read.
        configurationBuilder.Properties<DateTime>().HaveConversion<UtcDateTimeConverter>();
        configurationBuilder.Properties<DateTime?>().HaveConversion<NullableUtcDateTimeConverter>();
    }

    private sealed class UtcDateTimeConverter() : ValueConverter<DateTime, DateTime>(
        v => v.Kind == DateTimeKind.Utc ? v : v.ToUniversalTime(),
        v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

    private sealed class NullableUtcDateTimeConverter() : ValueConverter<DateTime?, DateTime?>(
        v => v.HasValue ? (v.Value.Kind == DateTimeKind.Utc ? v : v.Value.ToUniversalTime()) : v,
        v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : v);
}

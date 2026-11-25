using Microsoft.EntityFrameworkCore;
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

namespace NSBM.ICU.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    // Organisations
    DbSet<Company> Companies { get; }
    DbSet<PartnerEnquiry> PartnerEnquiries { get; }

    // People
    DbSet<Student> Students { get; }
    DbSet<Lecturer> Lecturers { get; }
    DbSet<IndustryPartner> IndustryPartners { get; }
    DbSet<Administrator> Administrators { get; }
    DbSet<Registrar> Registrars { get; }
    DbSet<StudentSkill> StudentSkills { get; }
    DbSet<CvProfile> CvProfiles { get; }
    DbSet<SkillBadge> SkillBadges { get; }
    DbSet<PlannerItem> PlannerItems { get; }

    // Careers catalogue
    DbSet<Skill> Skills { get; }
    DbSet<CareerPath> CareerPaths { get; }
    DbSet<CareerPathSkill> CareerPathSkills { get; }
    DbSet<RoadmapStep> RoadmapSteps { get; }
    DbSet<BaselineQuestion> BaselineQuestions { get; }

    // Learning progress
    DbSet<BaselineAttempt> BaselineAttempts { get; }
    DbSet<RoadmapStepCompletion> RoadmapStepCompletions { get; }
    DbSet<TaskAttempt> TaskAttempts { get; }
    DbSet<KnowledgeCheckSubmission> KnowledgeCheckSubmissions { get; }
    DbSet<InterviewAnswer> InterviewAnswers { get; }
    DbSet<TaskAttemptReview> TaskAttemptReviews { get; }
    DbSet<SprintItemProgress> SprintItemProgress { get; }

    // Mentoring and simulation
    DbSet<MentorQuestion> MentorQuestions { get; }
    DbSet<SprintWorkItem> SprintWorkItems { get; }

    // Lecturer support
    DbSet<SupportNote> SupportNotes { get; }
    DbSet<TaskAssignment> TaskAssignments { get; }

    // Interviews
    DbSet<InterviewQuestion> InterviewQuestions { get; }
    DbSet<AiInterviewSession> AiInterviewSessions { get; }

    // Industry tasks
    DbSet<IndustryTask> IndustryTasks { get; }

    // Opportunities
    DbSet<Vacancy> Vacancies { get; }
    DbSet<InternshipApplication> InternshipApplications { get; }

    // Unit calendar
    DbSet<CalendarEvent> CalendarEvents { get; }

    // Partner events
    DbSet<IndustryEvent> IndustryEvents { get; }
    DbSet<EventRegistration> EventRegistrations { get; }

    // Venues
    DbSet<Venue> Venues { get; }
    DbSet<VenueBooking> VenueBookings { get; }

    // Notifications
    DbSet<Notification> Notifications { get; }
    DbSet<Announcement> Announcements { get; }

    // Accountability
    DbSet<AuditEntry> AuditEntries { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

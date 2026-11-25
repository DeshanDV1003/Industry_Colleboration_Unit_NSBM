using NSBM.ICU.Application.Common.Models;

namespace NSBM.ICU.Application.Common.Auditing;

/// <summary>Every audited action code, in one place so filters and exports stay consistent.</summary>
public static class AuditActions
{
    public const string UserCreated = "user.created";
    public const string UserUpdated = "user.updated";
    public const string UserDeactivated = "user.deactivated";
    public const string UserReactivated = "user.reactivated";
    public const string PasswordReset = "user.password-reset";
    public const string PasswordChanged = "user.password-changed";
    public const string PasswordResetByLink = "user.password-reset-by-link";
    public const string StudentsImported = "user.students-imported";

    public const string TaskReviewed = "task.reviewed";
    public const string VacancyReviewed = "vacancy.reviewed";
    public const string EventReviewed = "event.reviewed";
    public const string BadgeReviewed = "badge.reviewed";

    public const string VenueCreated = "venue.created";
    public const string VenueUpdated = "venue.updated";
    public const string HallAllocated = "hall.allocated";
    public const string HallCancelled = "hall.cancelled";

    public const string AnnouncementSent = "announcement.sent";
    public const string CalendarEventCreated = "calendar.created";
    public const string CalendarEventUpdated = "calendar.updated";
    public const string CalendarEventDeleted = "calendar.deleted";

    public const string ApplicationStatusChanged = "application.status-changed";
    public const string CertificatesIssued = "event.certificates-issued";

    public const string DemoReset = "platform.demo-reset";
    public const string EnquiryHandled = "platform.enquiry-handled";
    public const string AdminProfileUpdated = "platform.admin-profile-updated";

    /// <summary>"Approved" or "Returned for changes", for review summaries.</summary>
    public static string Verdict(ReviewDecision decision) => decision == ReviewDecision.Approve ? "Approved" : "Returned for changes";
}

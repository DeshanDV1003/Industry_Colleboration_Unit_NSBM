namespace NSBM.ICU.Application.Common.Notifications;

/// <summary>
/// Web routes stored on notifications. Notifications are persisted messages, so they carry the page to open;
/// keeping every route here means a route change is a one-file edit.
/// </summary>
public static class NotificationLinks
{
    public const string Applications = "/applications";
    public const string MentorCenter = "/mentor-center";
    public const string Portfolio = "/portfolio";
    public const string Calendar = "/calendar";
    public const string PartnerTasks = "/industry";
    public const string PartnerVacancies = "/industry-hub";
    public const string TaskApprovals = "/admin";
    public const string VacancyApprovals = "/admin-vacancies";
    public const string EventApprovals = "/admin-events";
    public const string PartnerEvents = "/industry-events";
    public const string Certificates = "/certificates";
    public const string Achievements = "/achievements";
    public const string BadgeReviews = "/badge-reviews";
    public const string PartnerEnquiries = "/partner-enquiries";
    public const string HallBookings = "/hall-bookings";
    public const string Notifications = "/notifications";

    public static string Task(int taskId) => $"/tasks/{taskId}";
    public static string Event(int eventId) => $"/events/{eventId}";
}

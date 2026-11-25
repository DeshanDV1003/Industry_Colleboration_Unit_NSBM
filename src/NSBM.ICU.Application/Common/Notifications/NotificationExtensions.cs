using Microsoft.EntityFrameworkCore;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Domain.Constants;
using NSBM.ICU.Domain.Entities.Calendar;
using NSBM.ICU.Domain.Entities.Notifications;
using NSBM.ICU.Domain.Enums;

namespace NSBM.ICU.Application.Common.Notifications;

/// <summary>
/// Adds notifications to the context without saving, so they commit in the same SaveChanges as the change
/// they describe — a notification never exists for a change that failed.
/// </summary>
public static class NotificationExtensions
{
    public static void Notify(this IApplicationDbContext db, IEnumerable<string> userIds, NotificationKind kind,
        string title, string body, string? link, DateTime at)
    {
        foreach (var userId in userIds.Distinct())
        {
            db.Notifications.Add(new Notification
            {
                RecipientUserId = userId,
                Kind = kind,
                Title = Truncate(title, 150),
                Body = Truncate(body, 500),
                Link = link,
                CreatedAt = at,
            });
        }
    }

    public static void Notify(this IApplicationDbContext db, string userId, NotificationKind kind,
        string title, string body, string? link, DateTime at) =>
        db.Notify([userId], kind, title, body, link, at);

    public static async Task<string> StudentUserIdAsync(this IApplicationDbContext db, int studentId, CancellationToken ct) =>
        await db.Students.Where(s => s.Id == studentId).Select(s => s.UserId).SingleAsync(ct);

    /// <summary>Every partner account at the company, so whoever is on duty sees it.</summary>
    public static Task<List<string>> CompanyUserIdsAsync(this IApplicationDbContext db, int companyId, CancellationToken ct) =>
        db.IndustryPartners.Where(p => p.CompanyId == companyId).Select(p => p.UserId).ToListAsync(ct);

    public static Task<List<string>> AdminUserIdsAsync(this IApplicationDbContext db, CancellationToken ct) =>
        db.Administrators.Select(a => a.UserId).ToListAsync(ct);

    public static Task<List<string>> RegistrarUserIdsAsync(this IApplicationDbContext db, CancellationToken ct) =>
        db.Registrars.Select(r => r.UserId).ToListAsync(ct);

    /// <summary>Everyone a calendar event is visible to, decided by the same domain rule the calendar uses.</summary>
    public static async Task<IEnumerable<string>> CalendarAudienceUserIdsAsync(this IApplicationDbContext db, CalendarEvent calendarEvent, CancellationToken ct)
    {
        var students = await db.Students.Select(s => new { s.UserId, s.CareerPathId }).ToListAsync(ct);
        var lecturers = await db.Lecturers.Select(l => l.UserId).ToListAsync(ct);
        var partners = await db.IndustryPartners.Select(p => p.UserId).ToListAsync(ct);
        var registrars = await db.RegistrarUserIdsAsync(ct);

        return students.Where(s => calendarEvent.IsVisibleTo(Roles.Student, s.CareerPathId)).Select(s => s.UserId)
            .Concat(calendarEvent.IsVisibleTo(Roles.Lecturer, null) ? lecturers : [])
            .Concat(calendarEvent.IsVisibleTo(Roles.Industry, null) ? partners : [])
            .Concat(calendarEvent.IsVisibleTo(Roles.Registrar, null) ? registrars : []);
    }

    /// <summary>The first message a new account receives; it is also its welcome e-mail. It never contains the password.</summary>
    public static void NotifyWelcome(this IApplicationDbContext db, string userId, string firstName, string email) =>
        db.Notify(userId, NotificationKind.Welcome, "Welcome to the NSBM Industry Collaboration Unit",
            $"Hello {firstName}, your account is ready. Sign in with {email} and the temporary password the Unit gave you, then change it from the user menu.",
            NotificationLinks.Dashboard, DateTime.UtcNow);

    /// <summary>An account security alert. These are always e-mailed, even to people who turned e-mail updates off.</summary>
    public static void NotifySecurity(this IApplicationDbContext db, string userId, string title, string body) =>
        db.Notify(userId, NotificationKind.AccountSecurity, title, body, NotificationLinks.MySettings, DateTime.UtcNow);

    private static string Truncate(string text, int max) => text.Length <= max ? text : text[..(max - 1)] + "…";
}

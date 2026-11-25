using Microsoft.EntityFrameworkCore;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Application.Common.Notifications;
using NSBM.ICU.Domain.Entities.Venues;
using NSBM.ICU.Domain.Enums;

namespace NSBM.ICU.Application.Features.Venues;

/// <summary>A group told about a hall allocation, and the page that group opens.</summary>
public sealed record BookingRecipientGroup(IReadOnlyCollection<string> UserIds, string Link);

/// <summary>
/// Who hears about a hall allocation or its cancellation: the people attending (the cohort, the registered
/// students and the host company, or the calendar event's audience) and the Unit admins, who oversee every allocation.
/// </summary>
public static class VenueBookingRecipients
{
    public static async Task<IReadOnlyList<BookingRecipientGroup>> ForAsync(
        this IApplicationDbContext db, VenueBooking booking, string? excludeUserId, CancellationToken ct)
    {
        var groups = new List<BookingRecipientGroup>();

        switch (booking.Purpose)
        {
            case VenueBookingPurpose.Lecture:
                groups.Add(new(await db.Students
                    .Where(s => s.Programme == booking.Programme && s.AcademicYear == booking.AcademicYear)
                    .Select(s => s.UserId).ToListAsync(ct), NotificationLinks.Calendar));
                break;

            case VenueBookingPurpose.IndustryEvent when booking.IndustryEventId is { } eventId:
                groups.Add(new(await db.EventRegistrations.Where(r => r.EventId == eventId)
                    .Select(r => r.Student!.UserId).ToListAsync(ct), NotificationLinks.Event(eventId)));
                var companyId = await db.IndustryEvents.Where(e => e.Id == eventId).Select(e => e.CompanyId).SingleAsync(ct);
                groups.Add(new(await db.CompanyUserIdsAsync(companyId, ct), NotificationLinks.PartnerEvents));
                break;

            case VenueBookingPurpose.UnitEvent when booking.CalendarEventId is { } calendarEventId:
                var calendarEvent = await db.CalendarEvents.AsNoTracking().SingleAsync(e => e.Id == calendarEventId, ct);
                groups.Add(new((await db.CalendarAudienceUserIdsAsync(calendarEvent, ct)).ToList(), NotificationLinks.Calendar));
                break;
        }

        groups.Add(new(await db.AdminUserIdsAsync(ct), NotificationLinks.HallBookings));
        groups.Add(new(await db.RegistrarUserIdsAsync(ct), NotificationLinks.HallBookings));

        return groups
            .Select(g => g with { UserIds = g.UserIds.Where(id => id != excludeUserId).Distinct().ToList() })
            .Where(g => g.UserIds.Count > 0)
            .ToList();
    }

    public static void NotifyAll(this IApplicationDbContext db, IEnumerable<BookingRecipientGroup> groups,
        NotificationKind kind, string title, string body, DateTime at)
    {
        // One notification per person even when they belong to two groups (e.g. an admin who is also in the audience).
        var told = new HashSet<string>();
        foreach (var group in groups)
            db.Notify(group.UserIds.Where(told.Add), kind, title, body, group.Link, at);
    }
}

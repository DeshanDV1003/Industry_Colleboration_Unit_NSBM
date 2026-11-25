using Microsoft.EntityFrameworkCore;
using NSBM.ICU.Domain.Entities.Venues;
using NSBM.ICU.Domain.Enums;
using NSBM.ICU.Infrastructure.Persistence.Seed.Data;

namespace NSBM.ICU.Infrastructure.Persistence.Seed;

/// <summary>
/// Seeds the campus venues (missing ones only, so registrar edits survive restarts) and a sample timetable relative to
/// today: weekly lectures for the demo student's cohort and a hall for the next approved partner event.
/// Venue names are fictional demo data.
/// </summary>
public sealed class VenueSeeder(ApplicationDbContext db)
{
    public async Task SeedAsync(CancellationToken cancellationToken)
    {
        await SeedVenuesAsync(cancellationToken);
        await SeedBookingsAsync(cancellationToken);
    }

    private async Task SeedVenuesAsync(CancellationToken ct)
    {
        var existing = await db.Venues.Select(v => v.Code).ToListAsync(ct);
        db.Venues.AddRange(VenueData.Venues.Where(v => !existing.Contains(v.Code)).Select(v => new Venue
        {
            Code = v.Code, Name = v.Name, Building = v.Building, Type = v.Type, Capacity = v.Capacity, Facilities = v.Facilities,
        }));
        await db.SaveChangesAsync(ct);
    }

    private async Task SeedBookingsAsync(CancellationToken ct)
    {
        if (await db.VenueBookings.AnyAsync(ct))
            return;

        var registrar = await db.Registrars.OrderBy(r => r.Id).Select(r => r.FirstName + " " + r.LastName).FirstOrDefaultAsync(ct);
        if (registrar is null)
            return;

        var venues = await db.Venues.ToDictionaryAsync(v => v.Code, ct);
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var daysToMonday = ((int)DayOfWeek.Monday - (int)today.DayOfWeek + 7) % 7;
        var nextMonday = today.AddDays(daysToMonday == 0 ? 7 : daysToMonday);

        foreach (var lecture in VenueData.Lectures)
        {
            var cohortSize = await db.Students.CountAsync(s => s.Programme == lecture.Programme && s.AcademicYear == lecture.AcademicYear, ct);
            if (cohortSize == 0) continue;

            for (var week = 0; week < VenueData.LectureWeeks; week++)
            {
                db.VenueBookings.Add(new VenueBooking
                {
                    VenueId = venues[lecture.VenueCode].Id,
                    Purpose = VenueBookingPurpose.Lecture,
                    Title = lecture.Title,
                    Date = nextMonday.AddDays(lecture.DayOffset + 7 * week),
                    StartTime = lecture.Start,
                    EndTime = lecture.End,
                    Programme = lecture.Programme,
                    AcademicYear = lecture.AcademicYear,
                    Attendees = cohortSize,
                    BookedByName = registrar,
                });
            }
        }

        var nextEvent = await db.IndustryEvents
            .Where(e => e.Status == ModerationStatus.Approved && e.StartDate >= today)
            .OrderBy(e => e.StartDate)
            .FirstOrDefaultAsync(ct);
        if (nextEvent is not null)
        {
            db.VenueBookings.Add(new VenueBooking
            {
                VenueId = venues[VenueData.MainEventHall].Id,
                Purpose = VenueBookingPurpose.IndustryEvent,
                Title = nextEvent.Title,
                Date = nextEvent.StartDate,
                StartTime = new TimeOnly(9, 0),
                EndTime = new TimeOnly(13, 0),
                IndustryEventId = nextEvent.Id,
                Attendees = nextEvent.Capacity,
                BookedByName = registrar,
            });
        }

        await db.SaveChangesAsync(ct);
    }
}

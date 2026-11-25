using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using NSBM.ICU.Application.Common.Extensions;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Application.Features.Calendar.Dtos;
using NSBM.ICU.Application.Features.Calendar.Models;
using NSBM.ICU.Domain.Constants;
using NSBM.ICU.Domain.Entities.Calendar;
using NSBM.ICU.Domain.Entities.Opportunities;
using NSBM.ICU.Domain.Entities.People;
using NSBM.ICU.Domain.Enums;

namespace NSBM.ICU.Application.Features.Calendar;

/// <summary>
/// Builds a viewer's calendar for any date range: the Unit events meant for them plus their own dates.
/// Shared by the year calendar and the weekly digest.
/// </summary>
public sealed class CalendarEntryReader(IApplicationDbContext db, ICurrentUser currentUser)
{
    public async Task<IReadOnlyList<CalendarEntryDto>> ReadAsync(DateOnly from, DateOnly to, CancellationToken ct)
    {
        var role = currentUser.Role ?? string.Empty;

        var entries = new List<CalendarEntryDto>();
        int? studentPathId = null;

        switch (role)
        {
            case Roles.Student:
                var student = await db.GetCurrentStudentAsync(currentUser, ct);
                studentPathId = student.CareerPathId;
                entries.AddRange(await StudentDatesAsync(student.Id, studentPathId, from, to, ct));
                entries.AddRange(await StudentHallsAsync(student, from, to, ct));
                break;
            case Roles.Industry:
                var partner = await db.GetCurrentPartnerAsync(currentUser, ct);
                entries.AddRange(await VacancyClosingsAsync(v => v.CompanyId == partner.CompanyId, from, to, ct));
                break;
            case Roles.Admin:
                entries.AddRange(await VacancyClosingsAsync(v => v.Status == ModerationStatus.Approved, from, to, ct));
                break;
        }

        // Visibility is a domain rule, so the year's events are loaded and filtered in memory.
        var events = await db.CalendarEvents.AsNoTracking()
            .Include(e => e.CareerPath)
            .Where(e => e.StartDate <= to && e.EndDate >= from)
            .ToListAsync(ct);
        entries.AddRange(events.Where(e => e.IsVisibleTo(role, studentPathId)).Select(ToEntry));
        entries.AddRange(await PartnerEventsAsync(from, to, ct));

        return entries.OrderBy(e => e.StartDate).ThenBy(e => e.Source).ThenBy(e => e.Title).ToList();
    }

    private static CalendarEntryDto ToEntry(CalendarEvent e) => new(
        $"event-{e.Id}", CalendarEntrySource.UnitEvent, e.Title, e.Description, e.StartDate, e.EndDate,
        e.Id, e.Type, e.Audience, e.Location, e.CareerPathId, e.CareerPath?.Name, e.IndustryTaskId, e.VacancyId);

    /// <summary>Approved partner events are public within the platform, so every role sees them.</summary>
    private async Task<List<CalendarEntryDto>> PartnerEventsAsync(DateOnly from, DateOnly to, CancellationToken ct)
    {
        var hosted = await db.IndustryEvents.AsNoTracking()
            .Where(e => e.Status == ModerationStatus.Approved && e.StartDate <= to && e.EndDate >= from)
            .Select(e => new { e.Id, e.Title, e.Format, Company = e.Company!.Name, e.StartDate, e.EndDate, e.Location })
            .ToListAsync(ct);

        return hosted
            .Select(e => new CalendarEntryDto($"industry-event-{e.Id}", CalendarEntrySource.IndustryEvent, e.Title,
                $"{e.Format} hosted by {e.Company}.", e.StartDate, e.EndDate, Location: e.Location, IndustryEventId: e.Id))
            .ToList();
    }

    private async Task<IEnumerable<CalendarEntryDto>> StudentDatesAsync(int studentId, int? pathId, DateOnly from, DateOnly to, CancellationToken ct)
    {
        var start = from.ToDateTime(TimeOnly.MinValue);
        var end = to.AddDays(1).ToDateTime(TimeOnly.MinValue);

        var planner = await db.PlannerItems.AsNoTracking()
            .Where(p => p.StudentId == studentId && !p.IsDone && p.DueDate >= from && p.DueDate <= to)
            .Select(p => new { p.Id, p.Title, p.Notes, Due = p.DueDate!.Value })
            .ToListAsync(ct);

        var assigned = await db.TaskAssignments.AsNoTracking()
            .Where(a => a.StudentId == studentId && a.AssignedAt >= start && a.AssignedAt < end)
            .Select(a => new { a.Id, a.TaskId, a.Task!.Title, a.LecturerName, a.Note, a.AssignedAt })
            .ToListAsync(ct);

        var invites = await db.InternshipApplications.AsNoTracking()
            .Where(a => a.StudentId == studentId && a.Status == ApplicationStatus.InterviewInvited
                        && a.StatusChangedAt >= start && a.StatusChangedAt < end)
            .Select(a => new { a.Id, a.VacancyId, a.Vacancy!.Title, Company = a.Vacancy.Company!.Name, a.StatusChangedAt })
            .ToListAsync(ct);

        var closings = pathId is null
            ? []
            : await VacancyClosingsAsync(v => v.Status == ModerationStatus.Approved && v.CareerPathId == pathId, from, to, ct);

        return planner
            .Select(p => new CalendarEntryDto($"planner-{p.Id}", CalendarEntrySource.PlannerDeadline, p.Title, p.Notes, p.Due, p.Due))
            .Concat(assigned.Select(a =>
            {
                var day = DateOnly.FromDateTime(a.AssignedAt);
                var note = a.Note is null ? $"Suggested by {a.LecturerName}." : $"{a.LecturerName}: {a.Note}";
                return new CalendarEntryDto($"assigned-{a.Id}", CalendarEntrySource.AssignedTask, $"Task assigned: {a.Title}",
                    note, day, day, IndustryTaskId: a.TaskId);
            }))
            .Concat(invites.Select(i =>
            {
                var day = DateOnly.FromDateTime(i.StatusChangedAt);
                return new CalendarEntryDto($"invite-{i.Id}", CalendarEntrySource.InterviewInvite, $"Interview invitation: {i.Title}",
                    $"{i.Company} invited you to interview.", day, day, VacancyId: i.VacancyId);
            }))
            .Concat(closings);
    }

    /// <summary>Halls allocated to the student's cohort for lectures, and to partner events they registered for.</summary>
    private async Task<List<CalendarEntryDto>> StudentHallsAsync(Student student, DateOnly from, DateOnly to, CancellationToken ct)
    {
        var halls = await db.VenueBookings.AsNoTracking()
            .Where(b => b.Date >= from && b.Date <= to)
            .Where(b => (b.Purpose == VenueBookingPurpose.Lecture && b.Programme == student.Programme && b.AcademicYear == student.AcademicYear)
                        || (b.IndustryEventId != null && db.EventRegistrations.Any(r => r.EventId == b.IndustryEventId && r.StudentId == student.Id)))
            .Select(b => new { b.Id, b.Title, b.Date, b.StartTime, b.EndTime, Venue = b.Venue!.Code + " · " + b.Venue.Name, b.Venue.Building, b.IndustryEventId })
            .ToListAsync(ct);

        return halls
            .Select(b => new CalendarEntryDto($"hall-{b.Id}", CalendarEntrySource.HallBooking, b.Title,
                $"{b.StartTime:HH\\:mm}–{b.EndTime:HH\\:mm} in {b.Venue}, {b.Building}.", b.Date, b.Date,
                Location: b.Venue, IndustryEventId: b.IndustryEventId))
            .ToList();
    }

    private async Task<List<CalendarEntryDto>> VacancyClosingsAsync(
        Expression<Func<Vacancy, bool>> scope, DateOnly from, DateOnly to, CancellationToken ct)
    {
        var vacancies = await db.Vacancies.AsNoTracking()
            .Where(scope)
            .Where(v => v.ClosingDate >= from && v.ClosingDate <= to)
            .Select(v => new { v.Id, v.Title, Company = v.Company!.Name, v.ClosingDate })
            .ToListAsync(ct);

        return vacancies
            .Select(v => new CalendarEntryDto($"vacancy-{v.Id}", CalendarEntrySource.VacancyClosing, $"Applications close: {v.Title}",
                $"{v.Company} internship vacancy.", v.ClosingDate, v.ClosingDate, VacancyId: v.Id))
            .ToList();
    }
}

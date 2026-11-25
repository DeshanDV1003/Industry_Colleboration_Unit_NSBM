using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Application.Features.Analytics.Dtos;
using NSBM.ICU.Domain.Enums;
using NSBM.ICU.Domain.Rules;

namespace NSBM.ICU.Application.Features.Analytics.Queries.GetAnalytics;

/// <summary>The analytics dashboard for the last <see cref="Weeks"/> weeks (activity, applications, events) and the next four weeks (halls).</summary>
public sealed record GetAnalyticsQuery(int Weeks = 12) : IRequest<AnalyticsDto>;

public sealed class GetAnalyticsQueryValidator : AbstractValidator<GetAnalyticsQuery>
{
    public GetAnalyticsQueryValidator() => RuleFor(x => x.Weeks).InclusiveBetween(4, 52);
}

public sealed class GetAnalyticsQueryHandler(IApplicationDbContext db, IDateTime clock) : IRequestHandler<GetAnalyticsQuery, AnalyticsDto>
{
    public const int HallWeeks = 4;
    private const int TeachingHoursPerDay = 14; // VenueBookingRules: 07:00–21:00

    private sealed record Practice(int StudentId, DateTime At, string Kind);

    public async Task<AnalyticsDto> Handle(GetAnalyticsQuery request, CancellationToken ct)
    {
        var today = DateOnly.FromDateTime(clock.UtcNow);
        var firstWeek = StreakRules.WeekStart(today).AddDays(-7 * (request.Weeks - 1));
        var from = firstWeek.ToDateTime(TimeOnly.MinValue);

        var practice = await PracticeAsync(from, ct);
        var activity = await ActivityAsync(practice, firstWeek, request.Weeks, from, ct);
        var students = await db.Students.CountAsync(ct);
        var activeInPeriod = practice.Select(p => p.StudentId).Distinct().Count();

        return new AnalyticsDto(
            request.Weeks, students, activeInPeriod, activity.Sum(w => w.Applications), HallWeeks, PrivacyRules.MinGroupSize,
            activity,
            await PipelineAsync(from, ct),
            await EventsAsync(firstWeek, today, ct),
            await HallsAsync(today, ct),
            await ReadinessAsync(ct));
    }

    /// <summary>Everything that counts as practice, as (student, time) pairs since <paramref name="from"/>.</summary>
    private async Task<List<Practice>> PracticeAsync(DateTime from, CancellationToken ct)
    {
        var tasks = await db.TaskAttempts.Where(a => a.SubmittedAt >= from).Select(a => new Practice(a.StudentId, a.SubmittedAt, "task")).ToListAsync(ct);
        var interviews = await db.InterviewAnswers.Where(a => a.AnsweredAt >= from).Select(a => new Practice(a.StudentId, a.AnsweredAt, "interview")).ToListAsync(ct);
        var baselines = await db.BaselineAttempts.Where(a => a.CompletedAt >= from).Select(a => new Practice(a.StudentId, a.CompletedAt, "baseline")).ToListAsync(ct);
        var checks = await db.KnowledgeCheckSubmissions.Where(a => a.SubmittedAt >= from).Select(a => new Practice(a.StudentId, a.SubmittedAt, "check")).ToListAsync(ct);
        var roadmap = await db.RoadmapStepCompletions.Where(a => a.CompletedAt >= from).Select(a => new Practice(a.StudentId, a.CompletedAt, "roadmap")).ToListAsync(ct);
        return [.. tasks, .. interviews, .. baselines, .. checks, .. roadmap];
    }

    private async Task<List<WeekActivityDto>> ActivityAsync(List<Practice> practice, DateOnly firstWeek, int weeks, DateTime from, CancellationToken ct)
    {
        var applications = await db.InternshipApplications.Where(a => a.SubmittedAt >= from).Select(a => a.SubmittedAt).ToListAsync(ct);
        DateOnly Week(DateTime at) => StreakRules.WeekStart(DateOnly.FromDateTime(at));

        return Enumerable.Range(0, weeks).Select(i =>
        {
            var week = firstWeek.AddDays(7 * i);
            var inWeek = practice.Where(p => Week(p.At) == week).ToList();
            return new WeekActivityDto(week,
                inWeek.Select(p => p.StudentId).Distinct().Count(),
                inWeek.Count(p => p.Kind == "task"),
                inWeek.Count(p => p.Kind == "interview"),
                applications.Count(a => Week(a) == week));
        }).ToList();
    }

    private async Task<List<CompanyPipelineDto>> PipelineAsync(DateTime from, CancellationToken ct)
    {
        var rows = await db.InternshipApplications.Where(a => a.SubmittedAt >= from)
            .GroupBy(a => new { Company = a.Vacancy!.Company!.Name, a.Status })
            .Select(g => new { g.Key.Company, g.Key.Status, Count = g.Count() })
            .ToListAsync(ct);

        int Count(string company, ApplicationStatus status) => rows.Where(r => r.Company == company && r.Status == status).Sum(r => r.Count);
        return rows.Select(r => r.Company).Distinct()
            .Select(c => new CompanyPipelineDto(c, Count(c, ApplicationStatus.Submitted), Count(c, ApplicationStatus.UnderReview),
                Count(c, ApplicationStatus.Shortlisted), Count(c, ApplicationStatus.InterviewInvited), Count(c, ApplicationStatus.NotSelected)))
            .OrderByDescending(p => p.Total).ThenBy(p => p.Company)
            .ToList();
    }

    /// <summary>Approved events that have finished in the period, so attendance is final.</summary>
    private async Task<List<EventAttendanceDto>> EventsAsync(DateOnly from, DateOnly today, CancellationToken ct)
    {
        var events = await db.IndustryEvents
            .Where(e => e.Status == ModerationStatus.Approved && e.EndDate >= from && e.EndDate < today)
            .OrderBy(e => e.StartDate)
            .Select(e => new { e.Title, e.StartDate, Registered = e.Registrations.Count, Attended = e.Registrations.Count(r => r.Attended) })
            .ToListAsync(ct);
        return events.Select(e => new EventAttendanceDto(e.Title, e.StartDate, e.Registered, PrivacyRules.CanShowRate(e.Registered) ? e.Attended : null)).ToList();
    }

    private async Task<List<VenueUsageDto>> HallsAsync(DateOnly today, CancellationToken ct)
    {
        var end = today.AddDays(7 * HallWeeks);
        var weekdays = Enumerable.Range(0, 7 * HallWeeks).Select(today.AddDays).Count(d => d.DayOfWeek is not (DayOfWeek.Saturday or DayOfWeek.Sunday));
        var bookings = await db.VenueBookings.Where(b => b.Date >= today && b.Date < end)
            .Select(b => new { b.VenueId, b.StartTime, b.EndTime }).ToListAsync(ct);
        var venues = await db.Venues.Where(v => v.IsActive).OrderBy(v => v.Code).Select(v => new { v.Id, Name = v.Code + " · " + v.Name }).ToListAsync(ct);

        return venues.Select(v => new VenueUsageDto(v.Name,
                (int)Math.Round(bookings.Where(b => b.VenueId == v.Id).Sum(b => (b.EndTime - b.StartTime).TotalHours)),
                weekdays * TeachingHoursPerDay))
            .ToList();
    }

    private async Task<List<PathReadinessDto>> ReadinessAsync(CancellationToken ct)
    {
        var students = await db.Students.Where(s => s.CareerPathId != null)
            .Select(s => new
            {
                Path = s.CareerPath!.Name,
                Baseline = db.BaselineAttempts.Any(b => b.StudentId == s.Id && b.CareerPathId == s.CareerPathId),
                Done = db.RoadmapStepCompletions.Count(c => c.StudentId == s.Id && c.RoadmapStep!.CareerPathId == s.CareerPathId),
                Steps = db.RoadmapSteps.Count(r => r.CareerPathId == s.CareerPathId),
            })
            .ToListAsync(ct);

        return students.GroupBy(s => s.Path)
            .Select(g => PrivacyRules.CanShowRate(g.Count())
                ? new PathReadinessDto(g.Key, g.Count(), g.Count(s => s.Baseline), (int)Math.Round(g.Average(s => s.Steps == 0 ? 0 : 100.0 * s.Done / s.Steps)))
                : new PathReadinessDto(g.Key, g.Count(), null, null))
            .OrderBy(p => p.CareerPath)
            .ToList();
    }
}

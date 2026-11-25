using MediatR;
using Microsoft.EntityFrameworkCore;
using NSBM.ICU.Application.Common.Extensions;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Application.Features.Calendar;
using NSBM.ICU.Application.Features.Notifications.Dtos;
using NSBM.ICU.Domain.Constants;
using NSBM.ICU.Domain.Enums;

namespace NSBM.ICU.Application.Features.Notifications.Queries.GetWeeklyDigest;

/// <summary>
/// The "your week" digest, computed on request so it is always current: the next seven days of the
/// user's calendar, unread notifications, and role-specific next steps.
/// </summary>
public sealed record GetWeeklyDigestQuery : IRequest<WeeklyDigestDto>;

public sealed class GetWeeklyDigestQueryHandler(IApplicationDbContext db, ICurrentUser currentUser, IDateTime clock, CalendarEntryReader calendar)
    : IRequestHandler<GetWeeklyDigestQuery, WeeklyDigestDto>
{
    private const int DaysAhead = 7;
    private const int RecentUnreadShown = 5;

    public async Task<WeeklyDigestDto> Handle(GetWeeklyDigestQuery request, CancellationToken ct)
    {
        var from = DateOnly.FromDateTime(clock.UtcNow);
        var to = from.AddDays(DaysAhead - 1);

        var upcoming = await calendar.ReadAsync(from, to, ct);
        var mine = db.Notifications.AsNoTracking().For(currentUser.UserId).Where(n => n.ReadAt == null);
        var unreadCount = await mine.CountAsync(ct);
        var recentUnread = await mine.ToDtos().Take(RecentUnreadShown).ToListAsync(ct);

        var steps = currentUser.Role switch
        {
            Roles.Student => await StudentStepsAsync(from, ct),
            Roles.Admin => await AdminStepsAsync(ct),
            Roles.Registrar => await RegistrarStepsAsync(from, ct),
            _ => [],
        };

        return new WeeklyDigestDto(from, to, upcoming, unreadCount, recentUnread, steps);
    }

    private async Task<List<NextStepDto>> StudentStepsAsync(DateOnly today, CancellationToken ct)
    {
        var student = await db.GetCurrentStudentAsync(currentUser, ct);
        if (student.CareerPathId is not { } pathId)
            return [new NextStepDto(NextStepKind.ChooseCareerPath)];

        var steps = new List<NextStepDto>();
        if (!await db.BaselineAttempts.AnyAsync(b => b.StudentId == student.Id && b.CareerPathId == pathId, ct))
            steps.Add(new NextStepDto(NextStepKind.TakeBaseline));

        var nextRoadmapStep = await db.RoadmapSteps
            .Where(r => r.CareerPathId == pathId
                        && !db.RoadmapStepCompletions.Any(c => c.StudentId == student.Id && c.RoadmapStepId == r.Id))
            .OrderBy(r => r.SortOrder)
            .Select(r => r.Title)
            .FirstOrDefaultAsync(ct);
        if (nextRoadmapStep is not null)
            steps.Add(new NextStepDto(NextStepKind.ContinueRoadmap, nextRoadmapStep));

        var overdue = await db.PlannerItems.CountAsync(p => p.StudentId == student.Id && !p.IsDone && p.DueDate < today, ct);
        if (overdue > 0)
            steps.Add(new NextStepDto(NextStepKind.OverduePlannerItems, Count: overdue));

        var untriedAssignment = await db.TaskAssignments
            .Where(a => a.StudentId == student.Id
                        && !db.TaskAttempts.Any(t => t.StudentId == student.Id && t.TaskId == a.TaskId))
            .OrderByDescending(a => a.AssignedAt)
            .Select(a => new { a.TaskId, a.Task!.Title })
            .FirstOrDefaultAsync(ct);
        if (untriedAssignment is not null)
            steps.Add(new NextStepDto(NextStepKind.TryAssignedTask, untriedAssignment.Title, TargetId: untriedAssignment.TaskId));

        return steps;
    }

    private async Task<List<NextStepDto>> AdminStepsAsync(CancellationToken ct)
    {
        var pending = await db.IndustryTasks.CountAsync(t => t.Status == ModerationStatus.Pending, ct)
                      + await db.Vacancies.CountAsync(v => v.Status == ModerationStatus.Pending, ct);
        return pending > 0 ? [new NextStepDto(NextStepKind.ReviewPendingContent, Count: pending)] : [];
    }

    /// <summary>Upcoming approved partner events that have no hall yet.</summary>
    private async Task<List<NextStepDto>> RegistrarStepsAsync(DateOnly today, CancellationToken ct)
    {
        var withoutHall = await db.IndustryEvents.CountAsync(e => e.Status == ModerationStatus.Approved && e.EndDate >= today
                                                               && !db.VenueBookings.Any(b => b.IndustryEventId == e.Id), ct);
        return withoutHall > 0 ? [new NextStepDto(NextStepKind.AllocateEventHalls, Count: withoutHall)] : [];
    }
}

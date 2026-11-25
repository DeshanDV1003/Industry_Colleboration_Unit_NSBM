using MediatR;
using Microsoft.EntityFrameworkCore;
using NSBM.ICU.Application.Common.Extensions;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Application.Features.EventHub.Dtos;
using NSBM.ICU.Domain.Enums;

namespace NSBM.ICU.Application.Features.EventHub.Queries.GetEventHub;

/// <summary>
/// Approved events for students: everything upcoming, recent past events, and any past event the
/// student took part in. Upcoming events come first, soonest first.
/// </summary>
public sealed record GetEventHubQuery : IRequest<IReadOnlyList<EventCardDto>>;

public sealed class GetEventHubQueryHandler(IApplicationDbContext db, ICurrentUser currentUser, IDateTime clock)
    : IRequestHandler<GetEventHubQuery, IReadOnlyList<EventCardDto>>
{
    private const int RecentPastDays = 60;

    public async Task<IReadOnlyList<EventCardDto>> Handle(GetEventHubQuery request, CancellationToken ct)
    {
        var student = await db.GetCurrentStudentAsync(currentUser, ct);
        var today = DateOnly.FromDateTime(clock.UtcNow);
        var recent = today.AddDays(-RecentPastDays);

        var events = await db.IndustryEvents.AsNoTracking()
            .Include(e => e.Company)
            .Include(e => e.Registrations)
            .Where(e => e.Status == ModerationStatus.Approved
                        && (e.EndDate >= recent || e.Registrations.Any(r => r.StudentId == student.Id)))
            .ToListAsync(ct);

        return events
            .Select(e => e.ToCard(student.Id, today))
            .OrderBy(c => c.HasEnded)
            .ThenBy(c => c.HasEnded ? -c.StartDate.DayNumber : c.StartDate.DayNumber)
            .ToList();
    }
}

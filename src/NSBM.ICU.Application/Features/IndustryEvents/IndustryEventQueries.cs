using Microsoft.EntityFrameworkCore;
using NSBM.ICU.Application.Common.Exceptions;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Application.Features.IndustryEvents.Dtos;
using NSBM.ICU.Domain.Entities.Events;
using NSBM.ICU.Domain.Enums;

namespace NSBM.ICU.Application.Features.IndustryEvents;

/// <summary>Shared projections and loaders for industry events.</summary>
public static class IndustryEventQueries
{
    public static Task<List<IndustryEventListItemDto>> ToListItemsAsync(this IQueryable<IndustryEvent> source, CancellationToken ct) =>
        source.Select(e => new IndustryEventListItemDto(
                e.Id, e.Title, e.Company!.Name, e.Format, e.StartDate, e.EndDate, e.Capacity, e.Registrations.Count,
                e.MaxTeamSize != null, e.Status, e.CreatedAt, e.ReviewedAt, e.ReviewNote, e.Status != ModerationStatus.Approved))
            .ToListAsync(ct);

    /// <summary>The event with its registrations and teams, ready for aggregate methods.</summary>
    public static async Task<IndustryEvent> LoadEventAggregateAsync(this IApplicationDbContext db, int id, CancellationToken ct) =>
        await db.IndustryEvents
            .Include(e => e.Registrations)
            .Include(e => e.Teams)
            .FirstOrDefaultAsync(e => e.Id == id, ct)
        ?? throw NotFoundException.For("Event", id);
}

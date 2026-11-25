using MediatR;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Application.Features.IndustryEvents.Dtos;

namespace NSBM.ICU.Application.Features.IndustryEvents.Queries.GetEventReviewQueue;

/// <summary>Events from every company for the Unit's review board; the board filters by status.</summary>
public sealed record GetEventReviewQueueQuery : IRequest<IReadOnlyList<IndustryEventListItemDto>>;

public sealed class GetEventReviewQueueQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetEventReviewQueueQuery, IReadOnlyList<IndustryEventListItemDto>>
{
    public async Task<IReadOnlyList<IndustryEventListItemDto>> Handle(GetEventReviewQueueQuery request, CancellationToken ct) =>
        await db.IndustryEvents.OrderBy(e => e.CreatedAt).ToListItemsAsync(ct);
}

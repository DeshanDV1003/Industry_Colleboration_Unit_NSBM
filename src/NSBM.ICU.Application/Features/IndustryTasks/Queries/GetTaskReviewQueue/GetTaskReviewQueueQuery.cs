using MediatR;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Application.Features.IndustryTasks.Dtos;

namespace NSBM.ICU.Application.Features.IndustryTasks.Queries.GetTaskReviewQueue;

/// <summary>Tasks from every company for the Unit's review board.</summary>
public sealed record GetTaskReviewQueueQuery : IRequest<IReadOnlyList<IndustryTaskListItemDto>>;

public sealed class GetTaskReviewQueueQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetTaskReviewQueueQuery, IReadOnlyList<IndustryTaskListItemDto>>
{
    public async Task<IReadOnlyList<IndustryTaskListItemDto>> Handle(GetTaskReviewQueueQuery request, CancellationToken cancellationToken) =>
        await db.IndustryTasks.OrderBy(t => t.CreatedAt).ToListItemsAsync(db, cancellationToken);
}

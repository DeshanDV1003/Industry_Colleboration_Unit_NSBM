using MediatR;
using NSBM.ICU.Application.Common.Extensions;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Application.Features.IndustryTasks.Dtos;

namespace NSBM.ICU.Application.Features.IndustryTasks.Queries.GetCompanyTasks;

/// <summary>All tasks authored by the signed-in partner's company, newest first, with their review status.</summary>
public sealed record GetCompanyTasksQuery : IRequest<IReadOnlyList<IndustryTaskListItemDto>>;

public sealed class GetCompanyTasksQueryHandler(IApplicationDbContext db, ICurrentUser currentUser)
    : IRequestHandler<GetCompanyTasksQuery, IReadOnlyList<IndustryTaskListItemDto>>
{
    public async Task<IReadOnlyList<IndustryTaskListItemDto>> Handle(GetCompanyTasksQuery request, CancellationToken cancellationToken)
    {
        var partner = await db.GetCurrentPartnerAsync(currentUser, cancellationToken);

        return await db.IndustryTasks
            .Where(t => t.CompanyId == partner.CompanyId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListItemsAsync(db, cancellationToken);
    }
}

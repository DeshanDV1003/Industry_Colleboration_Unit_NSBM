using MediatR;
using NSBM.ICU.Application.Common.Extensions;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Application.Features.IndustryEvents.Dtos;

namespace NSBM.ICU.Application.Features.IndustryEvents.Queries.GetCompanyEvents;

/// <summary>The signed-in partner's company events, soonest first.</summary>
public sealed record GetCompanyEventsQuery : IRequest<IReadOnlyList<IndustryEventListItemDto>>;

public sealed class GetCompanyEventsQueryHandler(IApplicationDbContext db, ICurrentUser currentUser)
    : IRequestHandler<GetCompanyEventsQuery, IReadOnlyList<IndustryEventListItemDto>>
{
    public async Task<IReadOnlyList<IndustryEventListItemDto>> Handle(GetCompanyEventsQuery request, CancellationToken ct)
    {
        var partner = await db.GetCurrentPartnerAsync(currentUser, ct);
        return await db.IndustryEvents.Where(e => e.CompanyId == partner.CompanyId).OrderByDescending(e => e.StartDate).ToListItemsAsync(ct);
    }
}

using MediatR;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Application.Features.Vacancies.Dtos;

namespace NSBM.ICU.Application.Features.Vacancies.Queries.GetVacancyReviewQueue;

/// <summary>Vacancies from every company for the Unit's review board.</summary>
public sealed record GetVacancyReviewQueueQuery : IRequest<IReadOnlyList<VacancyListItemDto>>;

public sealed class GetVacancyReviewQueueQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetVacancyReviewQueueQuery, IReadOnlyList<VacancyListItemDto>>
{
    public async Task<IReadOnlyList<VacancyListItemDto>> Handle(GetVacancyReviewQueueQuery request, CancellationToken cancellationToken) =>
        await db.Vacancies.OrderBy(v => v.CreatedAt).ToListItemsAsync(db, cancellationToken);
}

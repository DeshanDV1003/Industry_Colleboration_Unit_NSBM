using MediatR;
using NSBM.ICU.Application.Common.Extensions;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Application.Features.Vacancies.Dtos;

namespace NSBM.ICU.Application.Features.Vacancies.Queries.GetCompanyVacancies;

/// <summary>All vacancies posted by the signed-in partner's company, newest first.</summary>
public sealed record GetCompanyVacanciesQuery : IRequest<IReadOnlyList<VacancyListItemDto>>;

public sealed class GetCompanyVacanciesQueryHandler(IApplicationDbContext db, ICurrentUser currentUser)
    : IRequestHandler<GetCompanyVacanciesQuery, IReadOnlyList<VacancyListItemDto>>
{
    public async Task<IReadOnlyList<VacancyListItemDto>> Handle(GetCompanyVacanciesQuery request, CancellationToken cancellationToken)
    {
        var partner = await db.GetCurrentPartnerAsync(currentUser, cancellationToken);

        return await db.Vacancies
            .Where(v => v.CompanyId == partner.CompanyId)
            .OrderByDescending(v => v.CreatedAt)
            .ToListItemsAsync(db, cancellationToken);
    }
}

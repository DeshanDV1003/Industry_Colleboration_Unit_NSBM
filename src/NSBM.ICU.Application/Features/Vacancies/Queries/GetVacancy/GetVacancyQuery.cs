using MediatR;
using Microsoft.EntityFrameworkCore;
using NSBM.ICU.Application.Common.Exceptions;
using NSBM.ICU.Application.Common.Extensions;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Application.Features.Vacancies.Dtos;

namespace NSBM.ICU.Application.Features.Vacancies.Queries.GetVacancy;

/// <summary>The complete vacancy. Visible to the posting company and to Unit admins.</summary>
public sealed record GetVacancyQuery(int Id) : IRequest<VacancyDto>;

public sealed class GetVacancyQueryHandler(IApplicationDbContext db, ICurrentUser currentUser)
    : IRequestHandler<GetVacancyQuery, VacancyDto>
{
    public async Task<VacancyDto> Handle(GetVacancyQuery request, CancellationToken cancellationToken)
    {
        var companyId = await db.Vacancies.Where(v => v.Id == request.Id).Select(v => (int?)v.CompanyId).FirstOrDefaultAsync(cancellationToken)
                        ?? throw NotFoundException.For("Vacancy", request.Id);

        await db.EnsureCompanyAccessAsync(currentUser, companyId, cancellationToken);

        return (await db.Vacancies.Where(v => v.Id == request.Id).ToDetailAsync(cancellationToken))!;
    }
}

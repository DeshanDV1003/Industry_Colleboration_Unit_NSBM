using MediatR;
using Microsoft.EntityFrameworkCore;
using NSBM.ICU.Application.Common.Extensions;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Application.Features.Candidates.Dtos;

namespace NSBM.ICU.Application.Features.Candidates.Queries.GetCompanyApplications;

/// <summary>Applications to the partner company's vacancies, optionally for one vacancy, newest first.</summary>
public sealed record GetCompanyApplicationsQuery(int? VacancyId) : IRequest<IReadOnlyList<CompanyApplicationDto>>;

public sealed class GetCompanyApplicationsQueryHandler(IApplicationDbContext db, ICurrentUser currentUser)
    : IRequestHandler<GetCompanyApplicationsQuery, IReadOnlyList<CompanyApplicationDto>>
{
    public async Task<IReadOnlyList<CompanyApplicationDto>> Handle(GetCompanyApplicationsQuery request, CancellationToken cancellationToken)
    {
        var partner = await db.GetCurrentPartnerAsync(currentUser, cancellationToken);

        var applications = db.InternshipApplications.AsNoTracking().Where(a => a.Vacancy!.CompanyId == partner.CompanyId);
        if (request.VacancyId is { } vacancyId)
            applications = applications.Where(a => a.VacancyId == vacancyId);

        return await applications
            .OrderByDescending(a => a.SubmittedAt)
            .Select(a => new CompanyApplicationDto(
                a.Id,
                a.VacancyId,
                a.Vacancy!.Title,
                a.StudentId,
                a.Student!.FirstName + " " + a.Student.LastName,
                a.Student.StudentNo,
                a.Student.Programme,
                a.Status,
                a.SubmittedAt,
                a.MatchPercentAtSubmission,
                a.CoverNote))
            .ToListAsync(cancellationToken);
    }
}

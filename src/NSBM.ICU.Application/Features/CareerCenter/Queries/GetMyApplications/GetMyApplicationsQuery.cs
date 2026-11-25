using MediatR;
using Microsoft.EntityFrameworkCore;
using NSBM.ICU.Application.Common.Extensions;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Application.Features.CareerCenter.Dtos;

namespace NSBM.ICU.Application.Features.CareerCenter.Queries.GetMyApplications;

/// <summary>The student's applications with their current status, most recently updated first.</summary>
public sealed record GetMyApplicationsQuery : IRequest<IReadOnlyList<MyApplicationDto>>;

public sealed class GetMyApplicationsQueryHandler(IApplicationDbContext db, ICurrentUser currentUser)
    : IRequestHandler<GetMyApplicationsQuery, IReadOnlyList<MyApplicationDto>>
{
    public async Task<IReadOnlyList<MyApplicationDto>> Handle(GetMyApplicationsQuery request, CancellationToken cancellationToken)
    {
        var student = await db.GetCurrentStudentAsync(currentUser, cancellationToken);

        return await db.InternshipApplications.AsNoTracking()
            .Where(a => a.StudentId == student.Id)
            .OrderByDescending(a => a.StatusChangedAt)
            .Select(a => new MyApplicationDto(
                a.Id,
                a.VacancyId,
                a.Vacancy!.Title,
                a.Vacancy.Company!.Name,
                a.Vacancy.WorkMode,
                a.Status,
                a.SubmittedAt,
                a.StatusChangedAt,
                a.MatchPercentAtSubmission,
                a.CoverNote))
            .ToListAsync(cancellationToken);
    }
}

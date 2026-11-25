using MediatR;
using Microsoft.EntityFrameworkCore;
using NSBM.ICU.Application.Common.Extensions;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Application.Features.Practice.Dtos;
using NSBM.ICU.Application.Features.TaskReviews.Dtos;

namespace NSBM.ICU.Application.Features.TaskReviews.Queries.GetAttemptsForReview;

/// <summary>Each student's latest attempt on the partner company's tasks, unreviewed first.</summary>
public sealed record GetAttemptsForReviewQuery : IRequest<IReadOnlyList<AttemptForReviewDto>>;

public sealed class GetAttemptsForReviewQueryHandler(IApplicationDbContext db, ICurrentUser currentUser)
    : IRequestHandler<GetAttemptsForReviewQuery, IReadOnlyList<AttemptForReviewDto>>
{
    public async Task<IReadOnlyList<AttemptForReviewDto>> Handle(GetAttemptsForReviewQuery request, CancellationToken cancellationToken)
    {
        var partner = await db.GetCurrentPartnerAsync(currentUser, cancellationToken);

        var attempts = await db.TaskAttempts.AsNoTracking()
            .Where(a => a.Task!.CompanyId == partner.CompanyId)
            .Where(a => a.AttemptNumber == db.TaskAttempts.Where(x => x.TaskId == a.TaskId && x.StudentId == a.StudentId).Max(x => x.AttemptNumber))
            .Select(a => new
            {
                a.Id, a.TaskId, TaskTitle = a.Task!.Title,
                StudentName = a.Student!.FirstName + " " + a.Student.LastName, a.Student.StudentNo,
                a.AttemptNumber, a.SubmittedAt, a.PassedChecks, a.TotalChecks, a.Code,
                Reviews = a.Reviews.OrderBy(r => r.ReviewedAt)
                    .Select(r => new AttemptReviewDto(r.ReviewerName, r.Comment, r.Improvements, r.ReviewedAt)).ToList(),
            })
            .ToListAsync(cancellationToken);

        return attempts
            .OrderBy(a => a.Reviews.Count > 0).ThenByDescending(a => a.SubmittedAt)
            .Select(a => new AttemptForReviewDto(a.Id, a.TaskId, a.TaskTitle, a.StudentName, a.StudentNo, a.AttemptNumber,
                a.SubmittedAt, a.PassedChecks, a.TotalChecks, a.Code, a.Reviews))
            .ToList();
    }
}

using MediatR;
using NSBM.ICU.Application.Common.Extensions;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Application.Features.Interviews.Dtos;

namespace NSBM.ICU.Application.Features.Interviews.Queries.GetInterviewReport;

/// <summary>The signed-in student's own interview report.</summary>
public sealed record GetInterviewReportQuery : IRequest<InterviewReportDto>;

public sealed class GetInterviewReportQueryHandler(IApplicationDbContext db, ICurrentUser currentUser, IDateTime clock)
    : IRequestHandler<GetInterviewReportQuery, InterviewReportDto>
{
    public async Task<InterviewReportDto> Handle(GetInterviewReportQuery request, CancellationToken cancellationToken)
    {
        var student = await db.GetCurrentStudentAsync(currentUser, cancellationToken);
        return await db.BuildInterviewReportAsync(student, clock.UtcNow, cancellationToken);
    }
}

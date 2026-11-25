using MediatR;
using Microsoft.EntityFrameworkCore;
using NSBM.ICU.Application.Common.Exceptions;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Application.Features.Interviews.Dtos;

namespace NSBM.ICU.Application.Features.Interviews.Queries.GetStudentInterviewReport;

/// <summary>A student's interview report for a lecturer, so they can give kind, useful feedback.</summary>
public sealed record GetStudentInterviewReportQuery(int StudentId) : IRequest<InterviewReportDto>;

public sealed class GetStudentInterviewReportQueryHandler(IApplicationDbContext db, IDateTime clock)
    : IRequestHandler<GetStudentInterviewReportQuery, InterviewReportDto>
{
    public async Task<InterviewReportDto> Handle(GetStudentInterviewReportQuery request, CancellationToken cancellationToken)
    {
        var student = await db.Students.AsNoTracking().FirstOrDefaultAsync(s => s.Id == request.StudentId, cancellationToken)
                      ?? throw NotFoundException.For("Student", request.StudentId);
        return await db.BuildInterviewReportAsync(student, clock.UtcNow, cancellationToken);
    }
}

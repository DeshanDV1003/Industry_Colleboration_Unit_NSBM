using MediatR;
using Microsoft.EntityFrameworkCore;
using NSBM.ICU.Application.Common.Extensions;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Application.Features.AiInterviews.Dtos;
using NSBM.ICU.Domain.Enums;

namespace NSBM.ICU.Application.Features.AiInterviews.Queries.GetMyAiInterviews;

/// <summary>The student's practice interviews, newest first, so they can see their progress over time.</summary>
public sealed record GetMyAiInterviewsQuery : IRequest<IReadOnlyList<AiInterviewSummaryDto>>;

public sealed class GetMyAiInterviewsQueryHandler(IApplicationDbContext db, ICurrentUser currentUser)
    : IRequestHandler<GetMyAiInterviewsQuery, IReadOnlyList<AiInterviewSummaryDto>>
{
    public async Task<IReadOnlyList<AiInterviewSummaryDto>> Handle(GetMyAiInterviewsQuery request, CancellationToken ct)
    {
        var student = await db.GetCurrentStudentAsync(currentUser, ct);
        return await db.AiInterviewSessions.AsNoTracking()
            .Where(s => s.StudentId == student.Id)
            .OrderByDescending(s => s.StartedAt)
            .Select(s => new AiInterviewSummaryDto(s.Id, s.Stage, s.TargetRole, s.Status, s.StartedAt, s.CompletedAt,
                s.Turns.Count(t => t.Speaker == AiTurnSpeaker.Student),
                s.Scores.Any() ? Math.Round(s.Scores.Average(x => (double)x.Score), 1) : null))
            .ToListAsync(ct);
    }
}

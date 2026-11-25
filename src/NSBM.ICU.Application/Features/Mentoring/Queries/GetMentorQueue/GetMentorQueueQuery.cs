using MediatR;
using Microsoft.EntityFrameworkCore;
using NSBM.ICU.Application.Common.Extensions;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Application.Features.Mentoring.Dtos;

namespace NSBM.ICU.Application.Features.Mentoring.Queries.GetMentorQueue;

/// <summary>All student questions for industry mentors: unanswered (oldest first), then answered.</summary>
public sealed record GetMentorQueueQuery : IRequest<IReadOnlyList<MentorQuestionDto>>;

public sealed class GetMentorQueueQueryHandler(IApplicationDbContext db, ICurrentUser currentUser)
    : IRequestHandler<GetMentorQueueQuery, IReadOnlyList<MentorQuestionDto>>
{
    public async Task<IReadOnlyList<MentorQuestionDto>> Handle(GetMentorQueueQuery request, CancellationToken cancellationToken)
    {
        await db.GetCurrentPartnerAsync(currentUser, cancellationToken);
        return await db.MentorQuestions.AsNoTracking()
            .OrderBy(q => q.Reply != null).ThenBy(q => q.AskedAt)
            .Select(MentorQuestionProjection.ToDto)
            .ToListAsync(cancellationToken);
    }
}

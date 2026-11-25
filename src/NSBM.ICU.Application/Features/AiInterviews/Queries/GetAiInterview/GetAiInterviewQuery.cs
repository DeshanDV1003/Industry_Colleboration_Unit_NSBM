using MediatR;
using NSBM.ICU.Application.Common.Extensions;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Application.Features.AiInterviews.Dtos;

namespace NSBM.ICU.Application.Features.AiInterviews.Queries.GetAiInterview;

/// <summary>One of the student's own practice interviews, with the conversation and any feedback.</summary>
public sealed record GetAiInterviewQuery(int SessionId) : IRequest<AiInterviewDto>;

public sealed class GetAiInterviewQueryHandler(IApplicationDbContext db, ICurrentUser currentUser) : IRequestHandler<GetAiInterviewQuery, AiInterviewDto>
{
    public async Task<AiInterviewDto> Handle(GetAiInterviewQuery request, CancellationToken ct)
    {
        var student = await db.GetCurrentStudentAsync(currentUser, ct);
        return (await db.LoadOwnSessionAsync(request.SessionId, student.Id, ct)).ToDto();
    }
}

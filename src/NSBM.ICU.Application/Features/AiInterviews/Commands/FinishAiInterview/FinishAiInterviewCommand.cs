using MediatR;
using NSBM.ICU.Application.Common.Extensions;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Application.Features.AiInterviews.Dtos;

namespace NSBM.ICU.Application.Features.AiInterviews.Commands.FinishAiInterview;

/// <summary>Ends the interview early and gets feedback on the answers given so far (at least one).</summary>
public sealed record FinishAiInterviewCommand(int SessionId) : IRequest<AiInterviewDto>;

public sealed class FinishAiInterviewCommandHandler(IApplicationDbContext db, ICurrentUser currentUser, AiInterviewConductor conductor)
    : IRequestHandler<FinishAiInterviewCommand, AiInterviewDto>
{
    public async Task<AiInterviewDto> Handle(FinishAiInterviewCommand request, CancellationToken ct)
    {
        var student = await db.GetCurrentStudentAsync(currentUser, ct);
        var session = await db.LoadOwnSessionAsync(request.SessionId, student.Id, ct);

        await conductor.CompleteAsync(session, ct);
        await db.SaveChangesAsync(ct);
        return session.ToDto();
    }
}

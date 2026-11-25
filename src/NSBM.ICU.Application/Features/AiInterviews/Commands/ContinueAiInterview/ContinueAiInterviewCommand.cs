using MediatR;
using NSBM.ICU.Application.Common.Exceptions;
using NSBM.ICU.Application.Common.Extensions;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Application.Features.AiInterviews.Dtos;

namespace NSBM.ICU.Application.Features.AiInterviews.Commands.ContinueAiInterview;

/// <summary>Retries the AI's reply after a saved answer, when the earlier attempt hit a service error.</summary>
public sealed record ContinueAiInterviewCommand(int SessionId) : IRequest<AiInterviewDto>;

public sealed class ContinueAiInterviewCommandHandler(IApplicationDbContext db, ICurrentUser currentUser, AiInterviewConductor conductor)
    : IRequestHandler<ContinueAiInterviewCommand, AiInterviewDto>
{
    public async Task<AiInterviewDto> Handle(ContinueAiInterviewCommand request, CancellationToken ct)
    {
        var student = await db.GetCurrentStudentAsync(currentUser, ct);
        var session = await db.LoadOwnSessionAsync(request.SessionId, student.Id, ct);
        if (!session.AwaitingInterviewer)
            throw new BusinessRuleException("This interview is not waiting for the AI.");

        await conductor.AdvanceAsync(session, ct);
        await db.SaveChangesAsync(ct);
        return session.ToDto();
    }
}

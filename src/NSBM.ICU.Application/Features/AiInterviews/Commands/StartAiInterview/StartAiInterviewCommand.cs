using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NSBM.ICU.Application.Common.Exceptions;
using NSBM.ICU.Application.Common.Extensions;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Application.Features.AiInterviews.Dtos;
using NSBM.ICU.Domain.Entities.Interviews;
using NSBM.ICU.Domain.Enums;

namespace NSBM.ICU.Application.Features.AiInterviews.Commands.StartAiInterview;

/// <summary>
/// Starts a practice interview and gets the AI's opening question. The student must agree that their typed
/// answers are sent to the AI service (no audio or video is ever sent).
/// </summary>
public sealed record StartAiInterviewCommand(InterviewStage Stage, bool ConsentToAiProcessing) : IRequest<AiInterviewDto>;

public sealed class StartAiInterviewCommandValidator : AbstractValidator<StartAiInterviewCommand>
{
    public StartAiInterviewCommandValidator()
    {
        RuleFor(x => x.Stage).IsInEnum();
        RuleFor(x => x.ConsentToAiProcessing).Equal(true).WithMessage("Agree to send your answers to the AI service to use the AI interviewer.");
    }
}

public sealed class StartAiInterviewCommandHandler(
    IApplicationDbContext db, ICurrentUser currentUser, IDateTime clock, IAiInterviewer interviewer, AiInterviewConductor conductor)
    : IRequestHandler<StartAiInterviewCommand, AiInterviewDto>
{
    private const string GeneralRole = "a graduate role in the software industry";
    private const int FocusSkillCount = 6;

    public async Task<AiInterviewDto> Handle(StartAiInterviewCommand request, CancellationToken ct)
    {
        if (!interviewer.IsAvailable)
            throw new AiServiceUnavailableException("The AI interviewer is not set up yet. Use the standard practice questions for now.");

        var student = await db.GetCurrentStudentAsync(currentUser, ct);
        var path = student.CareerPathId is { } pathId
            ? await db.CareerPaths.AsNoTracking()
                .Where(p => p.Id == pathId)
                .Select(p => new
                {
                    p.Name,
                    Skills = p.Skills.OrderBy(s => s.Importance).ThenBy(s => s.Skill!.Name).Select(s => s.Skill!.Name).Take(FocusSkillCount).ToList(),
                })
                .FirstOrDefaultAsync(ct)
            : null;

        var now = clock.UtcNow;
        var session = new AiInterviewSession
        {
            StudentId = student.Id,
            Stage = request.Stage,
            TargetRole = path?.Name ?? GeneralRole,
            FocusSkills = path is null ? string.Empty : string.Join(", ", path.Skills),
            StartedAt = now,
            ConsentedAt = now,
        };

        // The session is saved only once the opening question exists, so a service error leaves nothing half-made.
        await conductor.AdvanceAsync(session, ct);
        db.AiInterviewSessions.Add(session);
        await db.SaveChangesAsync(ct);
        return session.ToDto();
    }
}

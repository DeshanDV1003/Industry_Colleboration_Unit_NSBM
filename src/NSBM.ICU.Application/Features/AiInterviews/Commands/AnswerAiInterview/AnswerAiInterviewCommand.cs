using FluentValidation;
using MediatR;
using NSBM.ICU.Application.Common.Extensions;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Application.Features.AiInterviews.Dtos;
using NSBM.ICU.Domain.Rules;

namespace NSBM.ICU.Application.Features.AiInterviews.Commands.AnswerAiInterview;

/// <summary>
/// Saves the student's answer, then asks the AI for the next question (or the feedback after the last one).
/// The answer is saved first, so if the AI fails the student can continue without retyping it.
/// </summary>
public sealed record AnswerAiInterviewCommand(int SessionId, string Answer) : IRequest<AiInterviewDto>;

public sealed class AnswerAiInterviewCommandValidator : AbstractValidator<AnswerAiInterviewCommand>
{
    public AnswerAiInterviewCommandValidator()
    {
        RuleFor(x => x.SessionId).GreaterThan(0);
        RuleFor(x => x.Answer).NotEmpty().WithMessage("Write or say an answer first.").MaximumLength(AiInterviewRubric.MaxAnswerLength);
    }
}

public sealed class AnswerAiInterviewCommandHandler(IApplicationDbContext db, ICurrentUser currentUser, IDateTime clock, AiInterviewConductor conductor)
    : IRequestHandler<AnswerAiInterviewCommand, AiInterviewDto>
{
    public async Task<AiInterviewDto> Handle(AnswerAiInterviewCommand request, CancellationToken ct)
    {
        var student = await db.GetCurrentStudentAsync(currentUser, ct);
        var session = await db.LoadOwnSessionAsync(request.SessionId, student.Id, ct);

        session.Answer(request.Answer.Trim(), clock.UtcNow);
        await db.SaveChangesAsync(ct);

        await conductor.AdvanceAsync(session, ct);
        await db.SaveChangesAsync(ct);
        return session.ToDto();
    }
}

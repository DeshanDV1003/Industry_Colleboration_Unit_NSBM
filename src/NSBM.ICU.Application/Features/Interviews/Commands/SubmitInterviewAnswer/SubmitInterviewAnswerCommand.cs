using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NSBM.ICU.Application.Common.Exceptions;
using NSBM.ICU.Application.Common.Extensions;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Application.Features.Interviews.Dtos;
using NSBM.ICU.Domain.Entities.Learning;
using NSBM.ICU.Domain.Enums;
using NSBM.ICU.Domain.Rules;

namespace NSBM.ICU.Application.Features.Interviews.Commands.SubmitInterviewAnswer;

/// <summary>Saves a practice answer and returns simple topic feedback. Students can answer again as often as they like.</summary>
public sealed record SubmitInterviewAnswerCommand(int QuestionId, string Text, AnswerInputMode InputMode) : IRequest<AnswerFeedbackDto>;

public sealed class SubmitInterviewAnswerCommandValidator : AbstractValidator<SubmitInterviewAnswerCommand>
{
    public SubmitInterviewAnswerCommandValidator()
    {
        RuleFor(x => x.QuestionId).GreaterThan(0);
        RuleFor(x => x.Text).NotEmpty().WithMessage("Write or say your answer first.").MaximumLength(4000);
        RuleFor(x => x.InputMode).IsInEnum();
    }
}

public sealed class SubmitInterviewAnswerCommandHandler(IApplicationDbContext db, ICurrentUser currentUser, IDateTime clock)
    : IRequestHandler<SubmitInterviewAnswerCommand, AnswerFeedbackDto>
{
    public async Task<AnswerFeedbackDto> Handle(SubmitInterviewAnswerCommand request, CancellationToken cancellationToken)
    {
        var student = await db.GetCurrentStudentAsync(currentUser, cancellationToken);
        var question = await db.InterviewQuestions.AsNoTracking().FirstOrDefaultAsync(q => q.Id == request.QuestionId, cancellationToken)
                       ?? throw NotFoundException.For("Interview question", request.QuestionId);

        var text = request.Text.Trim();
        var answer = new InterviewAnswer
        {
            StudentId = student.Id,
            QuestionId = question.Id,
            Text = text,
            InputMode = request.InputMode,
            AnsweredAt = clock.UtcNow,
            WordCount = InterviewFeedbackRules.CountWords(text),
            MatchedKeywords = InterviewFeedback.JoinKeywords(InterviewFeedbackRules.MatchedKeywords(text, question.KeywordList)),
        };

        db.InterviewAnswers.Add(answer);
        await db.SaveChangesAsync(cancellationToken);

        return InterviewFeedback.Build(answer, question);
    }
}

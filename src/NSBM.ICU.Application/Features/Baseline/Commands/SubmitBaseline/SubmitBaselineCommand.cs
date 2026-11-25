using FluentValidation;
using MediatR;
using NSBM.ICU.Application.Common.Exceptions;
using NSBM.ICU.Application.Common.Extensions;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Application.Features.Baseline.Dtos;
using NSBM.ICU.Domain.Entities.Learning;

namespace NSBM.ICU.Application.Features.Baseline.Commands.SubmitBaseline;

public sealed record BaselineAnswerInput(int QuestionId, int OptionId);

/// <summary>Scores a full baseline attempt for the student's chosen path and stores it as practice evidence.</summary>
public sealed record SubmitBaselineCommand(IReadOnlyList<BaselineAnswerInput> Answers) : IRequest<BaselineResultDto>;

public sealed class SubmitBaselineCommandValidator : AbstractValidator<SubmitBaselineCommand>
{
    public SubmitBaselineCommandValidator()
    {
        RuleFor(x => x.Answers).NotEmpty();
        RuleFor(x => x.Answers)
            .Must(answers => answers.Select(a => a.QuestionId).Distinct().Count() == answers.Count)
            .WithMessage("Each question can be answered only once.");
    }
}

public sealed class SubmitBaselineCommandHandler(IApplicationDbContext db, ICurrentUser currentUser, IDateTime clock)
    : IRequestHandler<SubmitBaselineCommand, BaselineResultDto>
{
    public async Task<BaselineResultDto> Handle(SubmitBaselineCommand request, CancellationToken cancellationToken)
    {
        var student = await db.GetCurrentStudentAsync(currentUser, cancellationToken);
        var pathId = student.RequireCareerPathId();
        var questions = await db.LoadQuestionsAsync(pathId, cancellationToken);

        var answers = request.Answers.ToDictionary(a => a.QuestionId, a => a.OptionId);
        if (questions.Any(q => !answers.ContainsKey(q.Id)) || answers.Count != questions.Count)
            throw new BusinessRuleException("Answer every question in the baseline before submitting.");

        var attempt = new BaselineAttempt
        {
            StudentId = student.Id,
            CareerPathId = pathId,
            CompletedAt = clock.UtcNow,
            QuestionCount = questions.Count,
        };

        foreach (var question in questions)
        {
            var option = question.Options.FirstOrDefault(o => o.Id == answers[question.Id])
                         ?? throw new BusinessRuleException($"The selected option does not belong to question {question.Id}.");

            attempt.Answers.Add(new BaselineAnswer { QuestionId = question.Id, SelectedOptionId = option.Id, IsCorrect = option.IsCorrect });
        }
        attempt.CorrectCount = attempt.Answers.Count(a => a.IsCorrect);

        db.BaselineAttempts.Add(attempt);
        await db.SaveChangesAsync(cancellationToken);

        return BaselineResultBuilder.Build(attempt, questions);
    }
}

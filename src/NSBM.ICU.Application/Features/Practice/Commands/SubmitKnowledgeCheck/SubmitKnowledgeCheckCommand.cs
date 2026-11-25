using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NSBM.ICU.Application.Common.Exceptions;
using NSBM.ICU.Application.Common.Extensions;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Domain.Entities.Learning;

namespace NSBM.ICU.Application.Features.Practice.Commands.SubmitKnowledgeCheck;

public sealed record KnowledgeAnswerInput(int QuestionId, string Text);

/// <summary>Saves the student's written answers as practice evidence. Resubmitting replaces earlier answers.</summary>
public sealed record SubmitKnowledgeCheckCommand(int TaskId, IReadOnlyList<KnowledgeAnswerInput> Answers) : IRequest;

public sealed class SubmitKnowledgeCheckCommandValidator : AbstractValidator<SubmitKnowledgeCheckCommand>
{
    public const int MinimumAnswerLength = 20;

    public SubmitKnowledgeCheckCommandValidator()
    {
        RuleFor(x => x.TaskId).GreaterThan(0);
        RuleFor(x => x.Answers).NotEmpty();
        RuleFor(x => x.Answers)
            .Must(answers => answers.Select(a => a.QuestionId).Distinct().Count() == answers.Count)
            .WithMessage("Each question can be answered only once.");
        RuleForEach(x => x.Answers).ChildRules(answer =>
            answer.RuleFor(a => a.Text)
                .Must(text => (text?.Trim().Length ?? 0) >= MinimumAnswerLength)
                .WithMessage($"Explain each answer in at least {MinimumAnswerLength} characters.")
                .MaximumLength(4000));
    }
}

public sealed class SubmitKnowledgeCheckCommandHandler(IApplicationDbContext db, ICurrentUser currentUser, IDateTime clock)
    : IRequestHandler<SubmitKnowledgeCheckCommand>
{
    public async Task Handle(SubmitKnowledgeCheckCommand request, CancellationToken cancellationToken)
    {
        var student = await db.GetCurrentStudentAsync(currentUser, cancellationToken);
        await db.EnsureApprovedTaskExistsAsync(request.TaskId, cancellationToken);

        if (!await db.TaskAttempts.AnyAsync(a => a.TaskId == request.TaskId && a.StudentId == student.Id, cancellationToken))
            throw new BusinessRuleException("Submit an attempt in the workbench before the knowledge check.");

        var questionIds = await db.IndustryTasks
            .Where(t => t.Id == request.TaskId)
            .SelectMany(t => t.KnowledgeQuestions.Select(q => q.Id))
            .ToListAsync(cancellationToken);

        var answered = request.Answers.Select(a => a.QuestionId).ToHashSet();
        if (!answered.SetEquals(questionIds))
            throw new BusinessRuleException("Answer every follow-up question before submitting.");

        var submission = await db.KnowledgeCheckSubmissions
            .Include(k => k.Answers)
            .FirstOrDefaultAsync(k => k.TaskId == request.TaskId && k.StudentId == student.Id, cancellationToken);

        if (submission is null)
        {
            submission = new KnowledgeCheckSubmission { StudentId = student.Id, TaskId = request.TaskId };
            db.KnowledgeCheckSubmissions.Add(submission);
        }

        submission.SubmittedAt = clock.UtcNow;
        submission.Answers.Clear();
        foreach (var answer in request.Answers)
            submission.Answers.Add(new KnowledgeAnswer { QuestionId = answer.QuestionId, Text = answer.Text.Trim() });

        await db.SaveChangesAsync(cancellationToken);
    }
}

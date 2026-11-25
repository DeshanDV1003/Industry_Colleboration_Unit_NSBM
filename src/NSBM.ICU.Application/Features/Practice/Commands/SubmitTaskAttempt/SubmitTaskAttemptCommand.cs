using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NSBM.ICU.Application.Common.Extensions;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Application.Features.Practice.Dtos;
using NSBM.ICU.Domain.Entities.Learning;
using NSBM.ICU.Domain.Rules;

namespace NSBM.ICU.Application.Features.Practice.Commands.SubmitTaskAttempt;

/// <summary>Runs the task's pattern checks against the submitted code and records the attempt. The code is never executed.</summary>
public sealed record SubmitTaskAttemptCommand(int TaskId, string Code) : IRequest<TaskAttemptDto>;

public sealed class SubmitTaskAttemptCommandValidator : AbstractValidator<SubmitTaskAttemptCommand>
{
    public SubmitTaskAttemptCommandValidator()
    {
        RuleFor(x => x.TaskId).GreaterThan(0);
        RuleFor(x => x.Code).NotEmpty().WithMessage("Write your solution before submitting.").MaximumLength(20000);
    }
}

public sealed class SubmitTaskAttemptCommandHandler(IApplicationDbContext db, ICurrentUser currentUser, IDateTime clock)
    : IRequestHandler<SubmitTaskAttemptCommand, TaskAttemptDto>
{
    public async Task<TaskAttemptDto> Handle(SubmitTaskAttemptCommand request, CancellationToken cancellationToken)
    {
        var student = await db.GetCurrentStudentAsync(currentUser, cancellationToken);
        await db.EnsureApprovedTaskExistsAsync(request.TaskId, cancellationToken);

        var checks = await db.LoadChecksAsync(request.TaskId, cancellationToken);
        var previousAttempts = await db.TaskAttempts
            .Where(a => a.TaskId == request.TaskId && a.StudentId == student.Id)
            .MaxAsync(a => (int?)a.AttemptNumber, cancellationToken) ?? 0;

        var attempt = new TaskAttempt
        {
            StudentId = student.Id,
            TaskId = request.TaskId,
            AttemptNumber = previousAttempts + 1,
            Code = request.Code,
            SubmittedAt = clock.UtcNow,
            TotalChecks = checks.Count,
        };

        foreach (var check in checks)
            attempt.CheckResults.Add(new TaskCheckResult { CheckId = check.Id, Passed = PatternCheckRules.Passes(request.Code, check.Pattern, check.ShouldMatch) });
        attempt.PassedChecks = attempt.CheckResults.Count(r => r.Passed);

        db.TaskAttempts.Add(attempt);
        await db.SaveChangesAsync(cancellationToken);

        return attempt.ToDto(checks);
    }
}

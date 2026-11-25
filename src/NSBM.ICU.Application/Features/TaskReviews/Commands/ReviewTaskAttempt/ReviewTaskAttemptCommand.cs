using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NSBM.ICU.Application.Common.Exceptions;
using NSBM.ICU.Application.Common.Extensions;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Domain.Entities.Learning;
using NSBM.ICU.Application.Common.Notifications;
using NSBM.ICU.Domain.Enums;

namespace NSBM.ICU.Application.Features.TaskReviews.Commands.ReviewTaskAttempt;

/// <summary>The company that set a task leaves review comments and improvement ideas on a student's attempt.</summary>
public sealed record ReviewTaskAttemptCommand(int AttemptId, string Comment, string? Improvements) : IRequest;

public sealed class ReviewTaskAttemptCommandValidator : AbstractValidator<ReviewTaskAttemptCommand>
{
    public ReviewTaskAttemptCommandValidator()
    {
        RuleFor(x => x.AttemptId).GreaterThan(0);
        RuleFor(x => x.Comment).NotEmpty().MinimumLength(10).WithMessage("Write a review comment of at least 10 characters.").MaximumLength(2000);
        RuleFor(x => x.Improvements).MaximumLength(2000);
    }
}

public sealed class ReviewTaskAttemptCommandHandler(IApplicationDbContext db, ICurrentUser currentUser, IDateTime clock)
    : IRequestHandler<ReviewTaskAttemptCommand>
{
    public async Task Handle(ReviewTaskAttemptCommand request, CancellationToken cancellationToken)
    {
        var partner = await db.GetCurrentPartnerAsync(currentUser, cancellationToken);
        var attempt = await db.TaskAttempts.Where(a => a.Id == request.AttemptId)
                          .Select(a => new { a.StudentId, a.TaskId, a.Task!.Title, a.Task.CompanyId })
                          .FirstOrDefaultAsync(cancellationToken)
                      ?? throw NotFoundException.For("Attempt", request.AttemptId);

        if (attempt.CompanyId != partner.CompanyId)
            throw new ForbiddenAccessException("Only the company that set this task can review attempts on it.");

        db.TaskAttemptReviews.Add(new TaskAttemptReview
        {
            AttemptId = request.AttemptId,
            ReviewerPartnerId = partner.Id,
            ReviewerName = partner.FullName,
            Comment = request.Comment.Trim(),
            Improvements = string.IsNullOrWhiteSpace(request.Improvements) ? null : request.Improvements.Trim(),
            ReviewedAt = clock.UtcNow,
        });
        db.Notify(await db.StudentUserIdAsync(attempt.StudentId, cancellationToken), NotificationKind.TaskReview,
            $"Company feedback on {attempt.Title}", $"{partner.FullName} reviewed your attempt.",
            NotificationLinks.Task(attempt.TaskId), clock.UtcNow);
        await db.SaveChangesAsync(cancellationToken);
    }
}

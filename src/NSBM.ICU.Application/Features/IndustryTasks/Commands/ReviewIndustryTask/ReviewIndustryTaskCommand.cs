using MediatR;
using Microsoft.EntityFrameworkCore;
using NSBM.ICU.Application.Common.Auditing;
using NSBM.ICU.Application.Common.Exceptions;
using NSBM.ICU.Application.Common.Extensions;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Application.Common.Models;
using NSBM.ICU.Application.Common.Moderation;
using NSBM.ICU.Application.Common.Notifications;
using NSBM.ICU.Domain.Enums;

namespace NSBM.ICU.Application.Features.IndustryTasks.Commands.ReviewIndustryTask;

/// <summary>Approves a pending task (publishing it to students) or rejects it with a reason for the company.</summary>
public sealed record ReviewIndustryTaskCommand(int Id, ReviewDecision Decision, string? Note) : IRequest, IReviewCommand;

public sealed class ReviewIndustryTaskCommandValidator : ReviewCommandValidator<ReviewIndustryTaskCommand>;

public sealed class ReviewIndustryTaskCommandHandler(IApplicationDbContext db, ICurrentUser currentUser, IDateTime clock, IAuditTrail audit)
    : IRequestHandler<ReviewIndustryTaskCommand>
{
    public async Task Handle(ReviewIndustryTaskCommand request, CancellationToken cancellationToken)
    {
        var reviewer = await db.GetCurrentAdministratorAsync(currentUser, cancellationToken);
        var task = await db.IndustryTasks.FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken)
                   ?? throw NotFoundException.For("Task", request.Id);

        task.ApplyReview(request, reviewer.FullName, clock.UtcNow);
        var approved = request.Decision == ReviewDecision.Approve;
        db.Notify(await db.CompanyUserIdsAsync(task.CompanyId, cancellationToken), NotificationKind.ContentReviewed,
            approved ? $"Approved: {task.Title}" : $"Changes needed: {task.Title}",
            approved ? "Your industry task is now live for students." : $"The Unit asked for changes: {request.Note}",
            NotificationLinks.PartnerTasks, clock.UtcNow);
        await audit.RecordAsync(AuditCategory.Moderation, AuditActions.TaskReviewed,
            $"{AuditActions.Verdict(request.Decision)} the industry task \"{task.Title}\"" + (approved ? "" : $": {request.Note}"),
            "Task", task.Id.ToString(), cancellationToken);
        await db.SaveChangesAsync(cancellationToken);
    }
}

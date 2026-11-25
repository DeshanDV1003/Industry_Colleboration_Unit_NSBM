using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NSBM.ICU.Application.Common.Exceptions;
using NSBM.ICU.Application.Common.Extensions;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Domain.Entities.Support;
using NSBM.ICU.Domain.Enums;
using NSBM.ICU.Application.Common.Notifications;

namespace NSBM.ICU.Application.Features.Lecturing.Commands.AssignTask;

/// <summary>Adds an approved industry task to a student's workflow as a lecturer suggestion.</summary>
public sealed record AssignTaskCommand(int StudentId, int TaskId, string? Note) : IRequest<int>;

public sealed class AssignTaskCommandValidator : AbstractValidator<AssignTaskCommand>
{
    public AssignTaskCommandValidator()
    {
        RuleFor(x => x.StudentId).GreaterThan(0);
        RuleFor(x => x.TaskId).GreaterThan(0);
        RuleFor(x => x.Note).MaximumLength(500);
    }
}

public sealed class AssignTaskCommandHandler(IApplicationDbContext db, ICurrentUser currentUser, IDateTime clock) : IRequestHandler<AssignTaskCommand, int>
{
    public async Task<int> Handle(AssignTaskCommand request, CancellationToken ct)
    {
        var lecturer = await db.GetCurrentLecturerAsync(currentUser, ct);
        if (!await db.Students.AnyAsync(s => s.Id == request.StudentId, ct))
            throw NotFoundException.For("Student", request.StudentId);
        var taskTitle = await db.IndustryTasks.Where(t => t.Id == request.TaskId && t.Status == ModerationStatus.Approved)
                            .Select(t => t.Title).FirstOrDefaultAsync(ct)
                        ?? throw new BusinessRuleException("Only approved industry tasks can be assigned.");
        if (await db.TaskAssignments.AnyAsync(a => a.StudentId == request.StudentId && a.TaskId == request.TaskId, ct))
            throw new BusinessRuleException("This task is already assigned to the student.");

        var assignment = new TaskAssignment
        {
            StudentId = request.StudentId,
            TaskId = request.TaskId,
            LecturerId = lecturer.Id,
            LecturerName = lecturer.FullName,
            Note = string.IsNullOrWhiteSpace(request.Note) ? null : request.Note.Trim(),
            AssignedAt = clock.UtcNow,
        };
        db.TaskAssignments.Add(assignment);
        db.Notify(await db.StudentUserIdAsync(request.StudentId, ct), NotificationKind.TaskAssigned,
            $"Task assigned: {taskTitle}", $"{lecturer.FullName} suggested this task for you.", NotificationLinks.Task(request.TaskId), clock.UtcNow);
        await db.SaveChangesAsync(ct);
        return assignment.Id;
    }
}

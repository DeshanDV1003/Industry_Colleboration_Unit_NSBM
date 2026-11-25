using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NSBM.ICU.Application.Common.Exceptions;
using NSBM.ICU.Application.Common.Extensions;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Domain.Entities.Simulation;
using NSBM.ICU.Domain.Enums;

namespace NSBM.ICU.Application.Features.TeamSimulation.Commands.MoveSprintItem;

/// <summary>Picks up (first move) or moves a card in the student's own role to a new column.</summary>
public sealed record MoveSprintItemCommand(int WorkItemId, SprintItemStatus Status) : IRequest;

public sealed class MoveSprintItemCommandValidator : AbstractValidator<MoveSprintItemCommand>
{
    public MoveSprintItemCommandValidator()
    {
        RuleFor(x => x.WorkItemId).GreaterThan(0);
        RuleFor(x => x.Status).IsInEnum();
    }
}

public sealed class MoveSprintItemCommandHandler(IApplicationDbContext db, ICurrentUser currentUser, IDateTime clock) : IRequestHandler<MoveSprintItemCommand>
{
    public async Task Handle(MoveSprintItemCommand request, CancellationToken cancellationToken)
    {
        var student = await db.GetCurrentStudentAsync(currentUser, cancellationToken);
        var pathId = student.RequireCareerPathId();

        var item = await db.SprintWorkItems.FirstOrDefaultAsync(i => i.Id == request.WorkItemId, cancellationToken)
                   ?? throw NotFoundException.For("Work item", request.WorkItemId);
        if (item.CareerPathId != pathId)
            throw new BusinessRuleException("This card belongs to another role. Pick up cards for your own career path.");

        var progress = await db.SprintItemProgress.FirstOrDefaultAsync(p => p.StudentId == student.Id && p.WorkItemId == item.Id, cancellationToken);
        if (progress is null)
        {
            progress = new SprintItemProgress { StudentId = student.Id, WorkItemId = item.Id };
            db.SprintItemProgress.Add(progress);
        }

        progress.Status = request.Status;
        progress.UpdatedAt = clock.UtcNow;
        await db.SaveChangesAsync(cancellationToken);
    }
}

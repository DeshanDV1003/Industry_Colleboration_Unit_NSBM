using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NSBM.ICU.Application.Common.Exceptions;
using NSBM.ICU.Application.Common.Extensions;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Domain.Entities.Learning;

namespace NSBM.ICU.Application.Features.Roadmap.Commands.SetRoadmapStepCompletion;

/// <summary>Marks a step of the student's roadmap as done or not done. Idempotent.</summary>
public sealed record SetRoadmapStepCompletionCommand(int StepId, bool Completed) : IRequest;

public sealed class SetRoadmapStepCompletionCommandValidator : AbstractValidator<SetRoadmapStepCompletionCommand>
{
    public SetRoadmapStepCompletionCommandValidator()
    {
        RuleFor(x => x.StepId).GreaterThan(0);
    }
}

public sealed class SetRoadmapStepCompletionCommandHandler(IApplicationDbContext db, ICurrentUser currentUser, IDateTime clock)
    : IRequestHandler<SetRoadmapStepCompletionCommand>
{
    public async Task Handle(SetRoadmapStepCompletionCommand request, CancellationToken cancellationToken)
    {
        var student = await db.GetCurrentStudentAsync(currentUser, cancellationToken);
        var pathId = student.RequireCareerPathId();

        if (!await db.RoadmapSteps.AnyAsync(s => s.Id == request.StepId && s.CareerPathId == pathId, cancellationToken))
            throw NotFoundException.For("Roadmap step", request.StepId);

        var existing = await db.RoadmapStepCompletions
            .FirstOrDefaultAsync(c => c.StudentId == student.Id && c.RoadmapStepId == request.StepId, cancellationToken);

        switch (request.Completed, existing)
        {
            case (true, null):
                db.RoadmapStepCompletions.Add(new RoadmapStepCompletion
                {
                    StudentId = student.Id,
                    RoadmapStepId = request.StepId,
                    CompletedAt = clock.UtcNow,
                });
                break;
            case (false, not null):
                db.RoadmapStepCompletions.Remove(existing);
                break;
            default:
                return;
        }

        await db.SaveChangesAsync(cancellationToken);
    }
}

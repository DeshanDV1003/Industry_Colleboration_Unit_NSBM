using MediatR;
using NSBM.ICU.Application.Common.Interfaces;

namespace NSBM.ICU.Application.Features.Planner.Commands.DeletePlannerItem;

public sealed record DeletePlannerItemCommand(int Id) : IRequest;

public sealed class DeletePlannerItemCommandHandler(IApplicationDbContext db, ICurrentUser currentUser) : IRequestHandler<DeletePlannerItemCommand>
{
    public async Task Handle(DeletePlannerItemCommand request, CancellationToken cancellationToken)
    {
        var item = await db.GetOwnPlannerItemAsync(currentUser, request.Id, cancellationToken);
        db.PlannerItems.Remove(item);
        await db.SaveChangesAsync(cancellationToken);
    }
}

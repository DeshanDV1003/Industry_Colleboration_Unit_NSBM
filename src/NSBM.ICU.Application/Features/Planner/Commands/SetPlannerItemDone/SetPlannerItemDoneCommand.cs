using MediatR;
using NSBM.ICU.Application.Common.Interfaces;

namespace NSBM.ICU.Application.Features.Planner.Commands.SetPlannerItemDone;

public sealed record SetPlannerItemDoneCommand(int Id, bool Done) : IRequest;

public sealed class SetPlannerItemDoneCommandHandler(IApplicationDbContext db, ICurrentUser currentUser, IDateTime clock)
    : IRequestHandler<SetPlannerItemDoneCommand>
{
    public async Task Handle(SetPlannerItemDoneCommand request, CancellationToken cancellationToken)
    {
        var item = await db.GetOwnPlannerItemAsync(currentUser, request.Id, cancellationToken);
        item.SetDone(request.Done, clock.UtcNow);
        await db.SaveChangesAsync(cancellationToken);
    }
}

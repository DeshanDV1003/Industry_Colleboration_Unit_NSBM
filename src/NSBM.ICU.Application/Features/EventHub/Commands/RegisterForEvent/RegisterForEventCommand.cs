using MediatR;
using NSBM.ICU.Application.Common.Extensions;
using NSBM.ICU.Application.Common.Interfaces;

namespace NSBM.ICU.Application.Features.EventHub.Commands.RegisterForEvent;

/// <summary>Takes a place at an open event, while places remain.</summary>
public sealed record RegisterForEventCommand(int EventId) : IRequest;

public sealed class RegisterForEventCommandHandler(IApplicationDbContext db, ICurrentUser currentUser, IDateTime clock) : IRequestHandler<RegisterForEventCommand>
{
    public async Task Handle(RegisterForEventCommand request, CancellationToken ct)
    {
        var student = await db.GetCurrentStudentAsync(currentUser, ct);
        var industryEvent = await db.LoadApprovedEventAsync(request.EventId, ct);
        industryEvent.Register(student.Id, clock.UtcNow);
        await db.SaveChangesAsync(ct);
    }
}

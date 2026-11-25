using MediatR;
using NSBM.ICU.Application.Common.Extensions;
using NSBM.ICU.Application.Common.Interfaces;

namespace NSBM.ICU.Application.Features.EventHub.Commands.CancelEventRegistration;

/// <summary>Gives up a place before registration closes. The student also leaves their team.</summary>
public sealed record CancelEventRegistrationCommand(int EventId) : IRequest;

public sealed class CancelEventRegistrationCommandHandler(IApplicationDbContext db, ICurrentUser currentUser, IDateTime clock)
    : IRequestHandler<CancelEventRegistrationCommand>
{
    public async Task Handle(CancelEventRegistrationCommand request, CancellationToken ct)
    {
        var student = await db.GetCurrentStudentAsync(currentUser, ct);
        var industryEvent = await db.LoadApprovedEventAsync(request.EventId, ct);
        industryEvent.CancelRegistration(student.Id, DateOnly.FromDateTime(clock.UtcNow));
        await db.SaveChangesAsync(ct);
    }
}

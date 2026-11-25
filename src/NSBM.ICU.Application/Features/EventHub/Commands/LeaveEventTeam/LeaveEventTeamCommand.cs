using MediatR;
using NSBM.ICU.Application.Common.Extensions;
using NSBM.ICU.Application.Common.Interfaces;

namespace NSBM.ICU.Application.Features.EventHub.Commands.LeaveEventTeam;

/// <summary>Leaves the current team but keeps the event registration. An empty team is removed.</summary>
public sealed record LeaveEventTeamCommand(int EventId) : IRequest;

public sealed class LeaveEventTeamCommandHandler(IApplicationDbContext db, ICurrentUser currentUser, IDateTime clock) : IRequestHandler<LeaveEventTeamCommand>
{
    public async Task Handle(LeaveEventTeamCommand request, CancellationToken ct)
    {
        var student = await db.GetCurrentStudentAsync(currentUser, ct);
        var industryEvent = await db.LoadApprovedEventAsync(request.EventId, ct);
        industryEvent.LeaveTeam(student.Id, DateOnly.FromDateTime(clock.UtcNow));
        await db.SaveChangesAsync(ct);
    }
}

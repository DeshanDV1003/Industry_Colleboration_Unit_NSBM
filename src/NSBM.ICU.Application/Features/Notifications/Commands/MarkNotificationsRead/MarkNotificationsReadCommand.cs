using MediatR;
using Microsoft.EntityFrameworkCore;
using NSBM.ICU.Application.Common.Interfaces;

namespace NSBM.ICU.Application.Features.Notifications.Commands.MarkNotificationsRead;

/// <summary>Marks one notification (by id) or, when no id is given, all of the user's notifications as read.</summary>
public sealed record MarkNotificationsReadCommand(int? Id = null) : IRequest;

public sealed class MarkNotificationsReadCommandHandler(IApplicationDbContext db, ICurrentUser currentUser, IDateTime clock)
    : IRequestHandler<MarkNotificationsReadCommand>
{
    public async Task Handle(MarkNotificationsReadCommand request, CancellationToken ct)
    {
        // Scoping to the recipient means another user's id simply matches nothing.
        var unread = await db.Notifications.For(currentUser.UserId)
            .Where(n => n.ReadAt == null && (request.Id == null || n.Id == request.Id))
            .ToListAsync(ct);

        foreach (var notification in unread)
            notification.MarkRead(clock.UtcNow);
        await db.SaveChangesAsync(ct);
    }
}

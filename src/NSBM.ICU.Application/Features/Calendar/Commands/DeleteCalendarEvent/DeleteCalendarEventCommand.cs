using MediatR;
using Microsoft.EntityFrameworkCore;
using NSBM.ICU.Application.Common.Exceptions;
using NSBM.ICU.Application.Common.Extensions;
using NSBM.ICU.Application.Common.Interfaces;

namespace NSBM.ICU.Application.Features.Calendar.Commands.DeleteCalendarEvent;

public sealed record DeleteCalendarEventCommand(int Id) : IRequest;

public sealed class DeleteCalendarEventCommandHandler(IApplicationDbContext db, ICurrentUser currentUser) : IRequestHandler<DeleteCalendarEventCommand>
{
    public async Task Handle(DeleteCalendarEventCommand request, CancellationToken ct)
    {
        await db.GetCurrentAdministratorAsync(currentUser, ct);
        var calendarEvent = await db.CalendarEvents.FirstOrDefaultAsync(e => e.Id == request.Id, ct)
            ?? throw NotFoundException.For("Calendar event", request.Id);
        db.CalendarEvents.Remove(calendarEvent);
        await db.SaveChangesAsync(ct);
    }
}

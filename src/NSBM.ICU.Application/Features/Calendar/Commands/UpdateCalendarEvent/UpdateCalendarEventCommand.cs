using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NSBM.ICU.Application.Common.Auditing;
using NSBM.ICU.Application.Common.Exceptions;
using NSBM.ICU.Application.Common.Extensions;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Application.Features.Calendar.Models;
using NSBM.ICU.Domain.Enums;

namespace NSBM.ICU.Application.Features.Calendar.Commands.UpdateCalendarEvent;

public sealed record UpdateCalendarEventCommand(int Id, CalendarEventInput Event) : IRequest;

public sealed class UpdateCalendarEventCommandValidator : AbstractValidator<UpdateCalendarEventCommand>
{
    public UpdateCalendarEventCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.Event).NotNull().SetValidator(new CalendarEventInputValidator());
    }
}

public sealed class UpdateCalendarEventCommandHandler(IApplicationDbContext db, ICurrentUser currentUser, IAuditTrail audit) : IRequestHandler<UpdateCalendarEventCommand>
{
    public async Task Handle(UpdateCalendarEventCommand request, CancellationToken ct)
    {
        await db.GetCurrentAdministratorAsync(currentUser, ct);
        var calendarEvent = await db.CalendarEvents.FirstOrDefaultAsync(e => e.Id == request.Id, ct)
            ?? throw NotFoundException.For("Calendar event", request.Id);
        await db.EnsureLinksAsync(request.Event, ct);

        calendarEvent.Apply(request.Event);
        await audit.RecordAsync(AuditCategory.Communication, AuditActions.CalendarEventUpdated,
            $"Updated the calendar event \"{calendarEvent.Title}\"", "CalendarEvent", calendarEvent.Id.ToString(), ct);
        await db.SaveChangesAsync(ct);
    }
}

using FluentValidation;
using MediatR;
using NSBM.ICU.Application.Common.Auditing;
using NSBM.ICU.Application.Common.Extensions;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Application.Common.Notifications;
using NSBM.ICU.Application.Features.Calendar.Models;
using NSBM.ICU.Domain.Entities.Calendar;
using NSBM.ICU.Domain.Enums;

namespace NSBM.ICU.Application.Features.Calendar.Commands.CreateCalendarEvent;

/// <summary>The Unit admin adds an event, programme, task release or interview drive to the year calendar.</summary>
public sealed record CreateCalendarEventCommand(CalendarEventInput Event) : IRequest<int>;

public sealed class CreateCalendarEventCommandValidator : AbstractValidator<CreateCalendarEventCommand>
{
    public CreateCalendarEventCommandValidator() => RuleFor(x => x.Event).NotNull().SetValidator(new CalendarEventInputValidator());
}

public sealed class CreateCalendarEventCommandHandler(IApplicationDbContext db, ICurrentUser currentUser, IDateTime clock, IAuditTrail audit) : IRequestHandler<CreateCalendarEventCommand, int>
{
    public async Task<int> Handle(CreateCalendarEventCommand request, CancellationToken ct)
    {
        await db.GetCurrentAdministratorAsync(currentUser, ct);
        await db.EnsureLinksAsync(request.Event, ct);

        var calendarEvent = new CalendarEvent();
        calendarEvent.Apply(request.Event);
        db.CalendarEvents.Add(calendarEvent);

        // Only upcoming events are announced; back-filling past events stays quiet.
        if (calendarEvent.EndDate >= DateOnly.FromDateTime(clock.UtcNow))
            db.Notify(await db.CalendarAudienceUserIdsAsync(calendarEvent, ct), NotificationKind.CalendarEvent,
                $"New on the calendar: {calendarEvent.Title}", $"Starts {calendarEvent.StartDate:d MMM yyyy}.", NotificationLinks.Calendar, clock.UtcNow);

        await db.SaveChangesAsync(ct);
        await audit.RecordAsync(AuditCategory.Communication, AuditActions.CalendarEventCreated,
            $"Added \"{calendarEvent.Title}\" to the calendar ({calendarEvent.StartDate:d MMM yyyy})", "CalendarEvent", calendarEvent.Id.ToString(), ct);
        await db.SaveChangesAsync(ct);
        return calendarEvent.Id;
    }
}

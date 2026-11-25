using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NSBM.ICU.Application.Common.Exceptions;
using NSBM.ICU.Application.Common.Extensions;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Application.Features.Calendar.Models;

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

public sealed class UpdateCalendarEventCommandHandler(IApplicationDbContext db, ICurrentUser currentUser) : IRequestHandler<UpdateCalendarEventCommand>
{
    public async Task Handle(UpdateCalendarEventCommand request, CancellationToken ct)
    {
        await db.GetCurrentAdministratorAsync(currentUser, ct);
        var calendarEvent = await db.CalendarEvents.FirstOrDefaultAsync(e => e.Id == request.Id, ct)
            ?? throw NotFoundException.For("Calendar event", request.Id);
        await db.EnsureLinksAsync(request.Event, ct);

        calendarEvent.Apply(request.Event);
        await db.SaveChangesAsync(ct);
    }
}

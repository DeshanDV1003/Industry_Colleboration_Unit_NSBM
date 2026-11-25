using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NSBM.ICU.Application.Common.Exceptions;
using NSBM.ICU.Application.Common.Extensions;
using NSBM.ICU.Application.Common.Interfaces;

namespace NSBM.ICU.Application.Features.IndustryEvents.Commands.SetAttendance;

/// <summary>The host records whether a registered student came. Allowed once the event has started.</summary>
public sealed record SetAttendanceCommand(int EventId, int RegistrationId, bool Attended) : IRequest;

public sealed class SetAttendanceCommandValidator : AbstractValidator<SetAttendanceCommand>
{
    public SetAttendanceCommandValidator()
    {
        RuleFor(x => x.EventId).GreaterThan(0);
        RuleFor(x => x.RegistrationId).GreaterThan(0);
    }
}

public sealed class SetAttendanceCommandHandler(IApplicationDbContext db, ICurrentUser currentUser, IDateTime clock) : IRequestHandler<SetAttendanceCommand>
{
    public async Task Handle(SetAttendanceCommand request, CancellationToken ct)
    {
        var partner = await db.GetCurrentPartnerAsync(currentUser, ct);
        var industryEvent = await db.LoadEventAggregateAsync(request.EventId, ct);
        if (industryEvent.CompanyId != partner.CompanyId)
            throw new ForbiddenAccessException("This event belongs to another company.");
        if (!industryEvent.HasStarted(DateOnly.FromDateTime(clock.UtcNow)))
            throw new BusinessRuleException("Attendance can be recorded once the event has started.");

        var registration = industryEvent.Registrations.FirstOrDefault(r => r.Id == request.RegistrationId)
                           ?? throw NotFoundException.For("Registration", request.RegistrationId);
        registration.MarkAttendance(request.Attended);
        await db.SaveChangesAsync(ct);
    }
}

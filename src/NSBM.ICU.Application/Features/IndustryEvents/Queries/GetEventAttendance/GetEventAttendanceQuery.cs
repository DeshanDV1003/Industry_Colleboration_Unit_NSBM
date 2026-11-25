using MediatR;
using Microsoft.EntityFrameworkCore;
using NSBM.ICU.Application.Common.Exceptions;
using NSBM.ICU.Application.Common.Extensions;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Application.Features.IndustryEvents.Dtos;

namespace NSBM.ICU.Application.Features.IndustryEvents.Queries.GetEventAttendance;

/// <summary>The host company's attendance sheet for one of its events.</summary>
public sealed record GetEventAttendanceQuery(int EventId) : IRequest<EventAttendanceDto>;

public sealed class GetEventAttendanceQueryHandler(IApplicationDbContext db, ICurrentUser currentUser, IDateTime clock)
    : IRequestHandler<GetEventAttendanceQuery, EventAttendanceDto>
{
    public async Task<EventAttendanceDto> Handle(GetEventAttendanceQuery request, CancellationToken ct)
    {
        var partner = await db.GetCurrentPartnerAsync(currentUser, ct);
        var e = await db.IndustryEvents.AsNoTracking()
                    .Include(x => x.Registrations).ThenInclude(r => r.Student)
                    .Include(x => x.Registrations).ThenInclude(r => r.Team)
                    .FirstOrDefaultAsync(x => x.Id == request.EventId, ct)
                ?? throw NotFoundException.For("Event", request.EventId);
        if (e.CompanyId != partner.CompanyId)
            throw new ForbiddenAccessException("This event belongs to another company.");

        var today = DateOnly.FromDateTime(clock.UtcNow);
        var attendees = e.Registrations
            .OrderBy(r => r.Team?.Name ?? "~").ThenBy(r => r.Student!.LastName).ThenBy(r => r.Student!.FirstName)
            .Select(r => new EventAttendeeDto(r.Id, r.Student!.FullName, r.Student.StudentNo, r.Student.Programme,
                r.Team?.Name, r.RegisteredAt, r.Attended, r.CertificateCode))
            .ToList();
        return new EventAttendanceDto(e.Id, e.Title, e.StartDate, e.EndDate, e.HasStarted(today), e.HasEnded(today), attendees);
    }
}

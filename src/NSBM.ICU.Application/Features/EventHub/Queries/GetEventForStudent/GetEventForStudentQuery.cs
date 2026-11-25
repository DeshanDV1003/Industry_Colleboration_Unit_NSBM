using MediatR;
using Microsoft.EntityFrameworkCore;
using NSBM.ICU.Application.Common.Exceptions;
using NSBM.ICU.Application.Common.Extensions;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Application.Features.EventHub.Dtos;
using NSBM.ICU.Domain.Enums;

namespace NSBM.ICU.Application.Features.EventHub.Queries.GetEventForStudent;

/// <summary>An approved event with the student's registration, team and certificate.</summary>
public sealed record GetEventForStudentQuery(int Id) : IRequest<EventDetailDto>;

public sealed class GetEventForStudentQueryHandler(IApplicationDbContext db, ICurrentUser currentUser, IDateTime clock)
    : IRequestHandler<GetEventForStudentQuery, EventDetailDto>
{
    public async Task<EventDetailDto> Handle(GetEventForStudentQuery request, CancellationToken ct)
    {
        var student = await db.GetCurrentStudentAsync(currentUser, ct);
        var e = await db.IndustryEvents.AsNoTracking()
                    .Include(x => x.Company)
                    .Include(x => x.Teams)
                    .Include(x => x.Registrations).ThenInclude(r => r.Student)
                    .FirstOrDefaultAsync(x => x.Id == request.Id && x.Status == ModerationStatus.Approved, ct)
                ?? throw NotFoundException.For("Event", request.Id);

        var mine = e.RegistrationOf(student.Id);
        var myTeam = mine?.TeamId is { } teamId ? e.Teams.First(t => t.Id == teamId) : null;
        var myTeamDto = myTeam is null ? null : new MyTeamDto(
            myTeam.Id, myTeam.Name, myTeam.JoinCode, myTeam.CaptainStudentId == student.Id,
            e.Registrations.Where(r => r.TeamId == myTeam.Id).OrderBy(r => r.RegisteredAt)
                .Select(r => new TeamMemberDto(r.Student!.FullName, r.StudentId == myTeam.CaptainStudentId, r.StudentId == student.Id))
                .ToList());

        var teams = e.Teams.OrderBy(t => t.Name)
            .Select(t => new EventTeamSummaryDto(t.Id, t.Name, e.MemberCount(t.Id), e.MemberCount(t.Id) >= e.MaxTeamSize))
            .ToList();

        return new EventDetailDto(e.ToCard(student.Id, DateOnly.FromDateTime(clock.UtcNow)), e.Description, myTeamDto, teams, mine?.CertificateCode);
    }
}

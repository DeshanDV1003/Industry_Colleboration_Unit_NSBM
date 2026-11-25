using MediatR;
using Microsoft.EntityFrameworkCore;
using NSBM.ICU.Application.Common.Exceptions;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Application.Common.Models;
using NSBM.ICU.Application.Features.Auth.Dtos;
using NSBM.ICU.Domain.Constants;

namespace NSBM.ICU.Application.Features.Auth.Queries.GetCurrentUser;

public sealed record GetCurrentUserQuery : IRequest<CurrentUserDto>;

public sealed class GetCurrentUserQueryHandler(IApplicationDbContext db, IIdentityService identity, ICurrentUser currentUser)
    : IRequestHandler<GetCurrentUserQuery, CurrentUserDto>
{
    private const string UnitName = "Industry Collaboration Unit";

    private sealed record ProfileSummary(int Id, string? Headline, string? Organisation);

    public async Task<CurrentUserDto> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUser.UserId ?? throw new AuthenticationFailedException("You are not signed in.");
        var user = await identity.FindByIdAsync(userId, cancellationToken)
                   ?? throw new AuthenticationFailedException("Your account is no longer active.");

        var profile = await LoadProfileAsync(user, cancellationToken);

        return new CurrentUserDto(user.UserId, user.Email, user.FullName, user.Role,
            profile?.Id, profile?.Headline, profile?.Organisation);
    }

    private Task<ProfileSummary?> LoadProfileAsync(AuthenticatedUser user, CancellationToken ct) => user.Role switch
    {
        Roles.Student => db.Students.AsNoTracking()
            .Where(s => s.UserId == user.UserId)
            .Select(s => new ProfileSummary(s.Id, s.StudentNo + " · " + s.Programme, "Batch " + s.Batch))
            .FirstOrDefaultAsync(ct),

        Roles.Lecturer => db.Lecturers.AsNoTracking()
            .Where(l => l.UserId == user.UserId)
            .Select(l => new ProfileSummary(l.Id, l.Designation, l.Department))
            .FirstOrDefaultAsync(ct),

        Roles.Industry => db.IndustryPartners.AsNoTracking()
            .Where(p => p.UserId == user.UserId)
            .Select(p => new ProfileSummary(p.Id, p.JobTitle, p.Company!.Name))
            .FirstOrDefaultAsync(ct),

        Roles.Admin => db.Administrators.AsNoTracking()
            .Where(a => a.UserId == user.UserId)
            .Select(a => new ProfileSummary(a.Id, a.Designation, UnitName))
            .FirstOrDefaultAsync(ct),

        Roles.Registrar => db.Registrars.AsNoTracking()
            .Where(r => r.UserId == user.UserId)
            .Select(r => new ProfileSummary(r.Id, r.Designation, r.Office))
            .FirstOrDefaultAsync(ct),

        _ => Task.FromResult<ProfileSummary?>(null),
    };
}

using MediatR;
using Microsoft.EntityFrameworkCore;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Application.Features.Users.Dtos;
using NSBM.ICU.Domain.Constants;

namespace NSBM.ICU.Application.Features.Users.Queries.GetUsers;

/// <summary>Every account, optionally narrowed to one role and a search on name, e-mail or number. Active first, then by name.</summary>
public sealed record GetUsersQuery(string? Role, string? Search) : IRequest<IReadOnlyList<UserDto>>;

public sealed class GetUsersQueryHandler(IApplicationDbContext db, IIdentityService identity) : IRequestHandler<GetUsersQuery, IReadOnlyList<UserDto>>
{
    private sealed record Row(string UserId, string Role, string FullName, string Email, string Summary);

    public async Task<IReadOnlyList<UserDto>> Handle(GetUsersQuery request, CancellationToken ct)
    {
        var rows = new List<Row>();
        bool Wants(string role) => string.IsNullOrEmpty(request.Role) || request.Role == role;

        if (Wants(Roles.Student))
            rows.AddRange(await db.Students.AsNoTracking().Select(s => new Row(s.UserId, Roles.Student, s.FirstName + " " + s.LastName, s.Email,
                s.StudentNo + " · " + s.Programme + " · Year " + s.AcademicYear)).ToListAsync(ct));
        if (Wants(Roles.Lecturer))
            rows.AddRange(await db.Lecturers.AsNoTracking().Select(l => new Row(l.UserId, Roles.Lecturer, l.FirstName + " " + l.LastName, l.Email,
                l.StaffNo + " · " + l.Designation)).ToListAsync(ct));
        if (Wants(Roles.Industry))
            rows.AddRange(await db.IndustryPartners.AsNoTracking().Select(p => new Row(p.UserId, Roles.Industry, p.FirstName + " " + p.LastName, p.Email,
                p.JobTitle + " · " + p.Company!.Name)).ToListAsync(ct));
        if (Wants(Roles.Admin))
            rows.AddRange(await db.Administrators.AsNoTracking().Select(a => new Row(a.UserId, Roles.Admin, a.FirstName + " " + a.LastName, a.Email,
                a.Designation)).ToListAsync(ct));
        if (Wants(Roles.Registrar))
            rows.AddRange(await db.Registrars.AsNoTracking().Select(r => new Row(r.UserId, Roles.Registrar, r.FirstName + " " + r.LastName, r.Email,
                r.Designation)).ToListAsync(ct));

        var search = request.Search?.Trim();
        var statuses = await identity.GetAccountStatusesAsync(ct);

        return rows
            .Where(r => string.IsNullOrEmpty(search)
                        || r.FullName.Contains(search, StringComparison.OrdinalIgnoreCase)
                        || r.Email.Contains(search, StringComparison.OrdinalIgnoreCase)
                        || r.Summary.Contains(search, StringComparison.OrdinalIgnoreCase))
            .Select(r => new UserDto(r.UserId, r.Role, r.FullName, r.Email, statuses.TryGetValue(r.UserId, out var s) && s.IsActive, r.Summary))
            .OrderByDescending(u => u.IsActive).ThenBy(u => u.FullName)
            .ToList();
    }
}

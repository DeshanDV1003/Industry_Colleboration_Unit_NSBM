using MediatR;
using Microsoft.EntityFrameworkCore;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Application.Features.Users.Dtos;

namespace NSBM.ICU.Application.Features.Users.Queries.GetUserOptions;

public sealed record GetUserOptionsQuery : IRequest<UserOptionsDto>;

public sealed class GetUserOptionsQueryHandler(IApplicationDbContext db) : IRequestHandler<GetUserOptionsQuery, UserOptionsDto>
{
    public async Task<UserOptionsDto> Handle(GetUserOptionsQuery request, CancellationToken ct)
    {
        var companies = await db.Companies.AsNoTracking().OrderBy(c => c.Name).Select(c => new UserOptionDto(c.Id, c.Name)).ToListAsync(ct);
        var programmes = await db.Students.AsNoTracking().Select(s => s.Programme).Distinct().OrderBy(p => p).ToListAsync(ct);
        return new UserOptionsDto(companies, programmes);
    }
}

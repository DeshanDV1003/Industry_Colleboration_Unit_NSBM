using MediatR;
using Microsoft.EntityFrameworkCore;
using NSBM.ICU.Application.Common.Exceptions;
using NSBM.ICU.Application.Common.Extensions;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Application.Features.IndustryEvents.Dtos;

namespace NSBM.ICU.Application.Features.IndustryEvents.Queries.GetIndustryEvent;

/// <summary>One event for its own company (to edit) or a Unit admin (to review).</summary>
public sealed record GetIndustryEventQuery(int Id) : IRequest<IndustryEventDto>;

public sealed class GetIndustryEventQueryHandler(IApplicationDbContext db, ICurrentUser currentUser) : IRequestHandler<GetIndustryEventQuery, IndustryEventDto>
{
    public async Task<IndustryEventDto> Handle(GetIndustryEventQuery request, CancellationToken ct)
    {
        var e = await db.IndustryEvents.AsNoTracking()
                    .Include(x => x.Company)
                    .Include(x => x.Registrations)
                    .FirstOrDefaultAsync(x => x.Id == request.Id, ct)
                ?? throw NotFoundException.For("Event", request.Id);
        await db.EnsureCompanyAccessAsync(currentUser, e.CompanyId, ct);

        return new IndustryEventDto(e.Id, e.Title, e.Description, e.Company!.Name, e.Format, e.StartDate, e.EndDate, e.Location,
            e.Capacity, e.RegistrationClosesOn, e.MinTeamSize, e.MaxTeamSize, e.Registrations.Count, e.Status,
            e.ReviewedBy, e.ReviewNote, e.CanBeEdited, e.CanBeReviewed);
    }
}

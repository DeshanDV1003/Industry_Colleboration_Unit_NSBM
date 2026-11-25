using MediatR;
using Microsoft.EntityFrameworkCore;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Application.Features.Showcase.Dtos;

namespace NSBM.ICU.Application.Features.Showcase.Queries.GetPartnerEnquiries;

/// <summary>The Unit's inbox of partner enquiries: open ones first, newest first.</summary>
public sealed record GetPartnerEnquiriesQuery : IRequest<IReadOnlyList<PartnerEnquiryDto>>;

public sealed class GetPartnerEnquiriesQueryHandler(IApplicationDbContext db) : IRequestHandler<GetPartnerEnquiriesQuery, IReadOnlyList<PartnerEnquiryDto>>
{
    public async Task<IReadOnlyList<PartnerEnquiryDto>> Handle(GetPartnerEnquiriesQuery request, CancellationToken ct) =>
        await db.PartnerEnquiries.AsNoTracking()
            .OrderBy(e => e.IsHandled).ThenByDescending(e => e.SubmittedAt)
            .Select(e => new PartnerEnquiryDto(e.Id, e.OrganisationName, e.ContactName, e.Email, e.Phone, e.Interest, e.Message,
                e.SubmittedAt, e.IsHandled, e.HandledAt, e.HandledBy))
            .ToListAsync(ct);
}

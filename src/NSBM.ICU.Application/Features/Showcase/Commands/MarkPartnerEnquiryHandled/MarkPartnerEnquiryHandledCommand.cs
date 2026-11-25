using MediatR;
using Microsoft.EntityFrameworkCore;
using NSBM.ICU.Application.Common.Exceptions;
using NSBM.ICU.Application.Common.Extensions;
using NSBM.ICU.Application.Common.Interfaces;

namespace NSBM.ICU.Application.Features.Showcase.Commands.MarkPartnerEnquiryHandled;

/// <summary>The Unit marks an enquiry as followed up.</summary>
public sealed record MarkPartnerEnquiryHandledCommand(int Id) : IRequest;

public sealed class MarkPartnerEnquiryHandledCommandHandler(IApplicationDbContext db, ICurrentUser currentUser, IDateTime clock)
    : IRequestHandler<MarkPartnerEnquiryHandledCommand>
{
    public async Task Handle(MarkPartnerEnquiryHandledCommand request, CancellationToken ct)
    {
        var admin = await db.GetCurrentAdministratorAsync(currentUser, ct);
        var enquiry = await db.PartnerEnquiries.FirstOrDefaultAsync(e => e.Id == request.Id, ct)
                      ?? throw NotFoundException.For("Partner enquiry", request.Id);
        enquiry.MarkHandled(admin.FullName, clock.UtcNow);
        await db.SaveChangesAsync(ct);
    }
}

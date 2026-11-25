using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSBM.ICU.Application.Features.Showcase.Commands.MarkPartnerEnquiryHandled;
using NSBM.ICU.Application.Features.Showcase.Dtos;
using NSBM.ICU.Application.Features.Showcase.Queries.GetPartnerEnquiries;
using NSBM.ICU.Domain.Constants;

namespace NSBM.ICU.API.Controllers;

/// <summary>The Unit's inbox for partnership enquiries from the public website.</summary>
[Authorize(Roles = Roles.Admin)]
[Route(ApiRoutes.V1 + "/partner-enquiries")]
public sealed class PartnerEnquiriesController : ApiControllerBase
{
    [HttpGet]
    public Task<IReadOnlyList<PartnerEnquiryDto>> List(CancellationToken ct) => Mediator.Send(new GetPartnerEnquiriesQuery(), ct);

    [HttpPost("{id:int}/handled")]
    public async Task<IActionResult> MarkHandled(int id, CancellationToken ct)
    {
        await Mediator.Send(new MarkPartnerEnquiryHandledCommand(id), ct);
        return NoContent();
    }
}

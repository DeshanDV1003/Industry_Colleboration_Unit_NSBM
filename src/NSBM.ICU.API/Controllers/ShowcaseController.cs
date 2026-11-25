using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using NSBM.ICU.API.Extensions;
using NSBM.ICU.Application.Features.Showcase.Commands.SubmitPartnerEnquiry;
using NSBM.ICU.Application.Features.Showcase.Dtos;
using NSBM.ICU.Application.Features.Showcase.Queries.GetShowcase;

namespace NSBM.ICU.API.Controllers;

/// <summary>The public homepage for prospective students and companies. No sign-in.</summary>
[AllowAnonymous]
[Route(ApiRoutes.V1 + "/showcase")]
public sealed class ShowcaseController : ApiControllerBase
{
    [HttpGet]
    public Task<ShowcaseDto> Get(CancellationToken ct) => Mediator.Send(new GetShowcaseQuery(), ct);

    [HttpPost("partner-enquiries")]
    [EnableRateLimiting(PresentationServiceExtensions.PublicFormsRateLimit)]
    public async Task<IActionResult> SubmitEnquiry(SubmitPartnerEnquiryCommand body, CancellationToken ct)
    {
        await Mediator.Send(body, ct);
        return Accepted();
    }
}

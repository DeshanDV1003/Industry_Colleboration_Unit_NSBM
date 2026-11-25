using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSBM.ICU.Application.Features.EventHub.Dtos;
using NSBM.ICU.Application.Features.EventHub.Queries.VerifyCertificate;

namespace NSBM.ICU.API.Controllers;

/// <summary>Public certificate verification, so an employer can check a code a student shared.</summary>
[AllowAnonymous]
[Route(ApiRoutes.V1 + "/certificates")]
public sealed class CertificatesController : ApiControllerBase
{
    [HttpGet("{code}")]
    public Task<CertificateDto> Verify(string code, CancellationToken ct) => Mediator.Send(new VerifyCertificateQuery(code), ct);
}

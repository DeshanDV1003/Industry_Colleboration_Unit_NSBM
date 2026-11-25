using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSBM.ICU.Application.Features.Portfolio.Dtos;
using NSBM.ICU.Application.Features.Portfolio.Queries.GetMyPortfolio;
using NSBM.ICU.Domain.Constants;

namespace NSBM.ICU.API.Controllers;

[Authorize(Roles = Roles.Student)]
[Route(ApiRoutes.V1 + "/me/portfolio")]
public sealed class PortfolioController : ApiControllerBase
{
    [HttpGet]
    public Task<PortfolioDto> Get(CancellationToken ct) => Mediator.Send(new GetMyPortfolioQuery(), ct);
}

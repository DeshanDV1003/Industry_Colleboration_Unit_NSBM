using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSBM.ICU.Application.Features.Baseline.Commands.SubmitBaseline;
using NSBM.ICU.Application.Features.Baseline.Dtos;
using NSBM.ICU.Application.Features.Baseline.Queries.GetBaseline;
using NSBM.ICU.Domain.Constants;

namespace NSBM.ICU.API.Controllers;

[Authorize(Roles = Roles.Student)]
[Route(ApiRoutes.V1 + "/me/baseline")]
public sealed class BaselineController : ApiControllerBase
{
    [HttpGet]
    public Task<BaselineDto> Get(CancellationToken ct) =>
        Mediator.Send(new GetBaselineQuery(), ct);

    [HttpPost("attempts")]
    public Task<BaselineResultDto> Submit(SubmitBaselineCommand command, CancellationToken ct) =>
        Mediator.Send(command, ct);
}

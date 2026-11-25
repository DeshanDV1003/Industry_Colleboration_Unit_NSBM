using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSBM.ICU.Application.Features.Careers.Dtos;
using NSBM.ICU.Application.Features.Careers.Queries.GetSkills;

namespace NSBM.ICU.API.Controllers;

[Authorize]
[Route(ApiRoutes.V1 + "/skills")]
public sealed class SkillsController : ApiControllerBase
{
    [HttpGet]
    public Task<IReadOnlyList<SkillDto>> List(CancellationToken ct) =>
        Mediator.Send(new GetSkillsQuery(), ct);
}

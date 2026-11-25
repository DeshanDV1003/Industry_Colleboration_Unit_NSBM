using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSBM.ICU.Application.Features.CvStudio.Commands.SaveMyCv;
using NSBM.ICU.Application.Features.CvStudio.Dtos;
using NSBM.ICU.Application.Features.CvStudio.Queries.FindCvKeywords;
using NSBM.ICU.Application.Features.CvStudio.Queries.GetMyCv;
using NSBM.ICU.Domain.Constants;

namespace NSBM.ICU.API.Controllers;

[Authorize(Roles = Roles.Student)]
[Route(ApiRoutes.V1 + "/me/cv")]
public sealed class CvStudioController : ApiControllerBase
{
    [HttpGet]
    public Task<CvDto> Get(CancellationToken ct) => Mediator.Send(new GetMyCvQuery(), ct);

    [HttpPut]
    public async Task<IActionResult> Save(SaveMyCvCommand command, CancellationToken ct)
    {
        await Mediator.Send(command, ct);
        return NoContent();
    }

    /// <summary>Finds catalogue skills in pasted CV text. POST because the text can be long.</summary>
    [HttpPost("keywords")]
    public Task<IReadOnlyList<CvKeywordMatchDto>> FindKeywords(FindCvKeywordsQuery query, CancellationToken ct) => Mediator.Send(query, ct);
}

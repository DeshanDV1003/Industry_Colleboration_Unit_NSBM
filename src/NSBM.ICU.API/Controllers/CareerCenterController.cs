using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSBM.ICU.Application.Features.CareerCenter.Commands.ApplyToVacancy;
using NSBM.ICU.Application.Features.CareerCenter.Dtos;
using NSBM.ICU.Application.Features.CareerCenter.Queries.GetMyApplications;
using NSBM.ICU.Application.Features.CareerCenter.Queries.SearchVacancies;
using NSBM.ICU.Domain.Constants;
using NSBM.ICU.Domain.Enums;

namespace NSBM.ICU.API.Controllers;

/// <summary>Students searching open vacancies, applying and tracking their applications.</summary>
[Authorize(Roles = Roles.Student)]
[Route(ApiRoutes.V1 + "/career-center")]
public sealed class CareerCenterController : ApiControllerBase
{
    public sealed record ApplyRequest(string CoverNote);

    [HttpGet("vacancies")]
    public Task<IReadOnlyList<VacancyMatchDto>> Search(
        [FromQuery] int? careerPathId, [FromQuery] WorkMode? workMode, [FromQuery] string? search, CancellationToken ct) =>
        Mediator.Send(new SearchVacanciesQuery(careerPathId, workMode, search), ct);

    [HttpPost("vacancies/{id:int}/applications")]
    public async Task<IActionResult> Apply(int id, ApplyRequest body, CancellationToken ct)
    {
        var applicationId = await Mediator.Send(new ApplyToVacancyCommand(id, body.CoverNote), ct);
        return Created($"/{ApiRoutes.V1}/career-center/applications", new { id = applicationId });
    }

    [HttpGet("applications")]
    public Task<IReadOnlyList<MyApplicationDto>> MyApplications(CancellationToken ct) =>
        Mediator.Send(new GetMyApplicationsQuery(), ct);
}

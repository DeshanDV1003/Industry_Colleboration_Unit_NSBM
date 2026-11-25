using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSBM.ICU.Application.Features.Dashboard.Dtos;
using NSBM.ICU.Application.Features.Dashboard.Queries.GetStudentOverview;
using NSBM.ICU.Domain.Constants;

namespace NSBM.ICU.API.Controllers;

[Authorize]
[Route(ApiRoutes.V1 + "/dashboard")]
public sealed class DashboardController : ApiControllerBase
{
    [Authorize(Roles = Roles.Student)]
    [HttpGet("student")]
    public Task<StudentOverviewDto> Student(CancellationToken ct) =>
        Mediator.Send(new GetStudentOverviewQuery(), ct);
}

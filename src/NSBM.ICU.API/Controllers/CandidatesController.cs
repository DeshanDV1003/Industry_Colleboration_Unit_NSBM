using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSBM.ICU.Application.Features.Candidates.Commands.UpdateApplicationStatus;
using NSBM.ICU.Application.Features.Candidates.Dtos;
using NSBM.ICU.Application.Features.Candidates.Queries.GetCandidateEvidence;
using NSBM.ICU.Application.Features.Candidates.Queries.GetCompanyApplications;
using NSBM.ICU.Application.Features.Candidates.Queries.GetSharedProfiles;
using NSBM.ICU.Domain.Constants;
using NSBM.ICU.Domain.Enums;

namespace NSBM.ICU.API.Controllers;

/// <summary>Industry partners reviewing applicants and opted-in student profiles.</summary>
[Authorize(Roles = Roles.Industry)]
[Route(ApiRoutes.V1 + "/candidates")]
public sealed class CandidatesController : ApiControllerBase
{
    public sealed record StatusRequest(ApplicationStatus Status);

    [HttpGet("applications")]
    public Task<IReadOnlyList<CompanyApplicationDto>> Applications([FromQuery] int? vacancyId, CancellationToken ct) =>
        Mediator.Send(new GetCompanyApplicationsQuery(vacancyId), ct);

    [HttpPut("applications/{id:int}/status")]
    public async Task<IActionResult> UpdateStatus(int id, StatusRequest body, CancellationToken ct)
    {
        await Mediator.Send(new UpdateApplicationStatusCommand(id, body.Status), ct);
        return NoContent();
    }

    [HttpGet("shared-profiles")]
    public Task<IReadOnlyList<SharedProfileDto>> SharedProfiles(CancellationToken ct) =>
        Mediator.Send(new GetSharedProfilesQuery(), ct);

    [HttpGet("evidence")]
    public Task<CandidateEvidenceDto> Evidence([FromQuery] int studentId, [FromQuery] int vacancyId, CancellationToken ct) =>
        Mediator.Send(new GetCandidateEvidenceQuery(studentId, vacancyId), ct);
}

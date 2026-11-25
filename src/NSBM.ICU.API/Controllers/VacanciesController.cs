using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSBM.ICU.API.Controllers.Requests;
using NSBM.ICU.Application.Features.Vacancies.Commands.CreateVacancy;
using NSBM.ICU.Application.Features.Vacancies.Commands.ReviewVacancy;
using NSBM.ICU.Application.Features.Vacancies.Commands.UpdateVacancy;
using NSBM.ICU.Application.Features.Vacancies.Dtos;
using NSBM.ICU.Application.Features.Vacancies.Models;
using NSBM.ICU.Application.Features.Vacancies.Queries.GetCompanyVacancies;
using NSBM.ICU.Application.Features.Vacancies.Queries.GetVacancy;
using NSBM.ICU.Application.Features.Vacancies.Queries.GetVacancyReviewQueue;
using NSBM.ICU.Domain.Constants;

namespace NSBM.ICU.API.Controllers;

/// <summary>Posting (industry partners) and moderation (Unit admins) of internship vacancies.</summary>
[Authorize]
[Route(ApiRoutes.V1 + "/vacancies")]
public sealed class VacanciesController : ApiControllerBase
{
    [Authorize(Roles = Roles.Industry)]
    [HttpGet("mine")]
    public Task<IReadOnlyList<VacancyListItemDto>> Mine(CancellationToken ct) =>
        Mediator.Send(new GetCompanyVacanciesQuery(), ct);

    [Authorize(Roles = Roles.Admin)]
    [HttpGet("review-queue")]
    public Task<IReadOnlyList<VacancyListItemDto>> ReviewQueue(CancellationToken ct) =>
        Mediator.Send(new GetVacancyReviewQueueQuery(), ct);

    [Authorize(Roles = Roles.Industry + "," + Roles.Admin)]
    [HttpGet("{id:int}")]
    public Task<VacancyDto> Get(int id, CancellationToken ct) =>
        Mediator.Send(new GetVacancyQuery(id), ct);

    [Authorize(Roles = Roles.Industry)]
    [HttpPost]
    public async Task<IActionResult> Create(VacancyInput vacancy, CancellationToken ct)
    {
        var id = await Mediator.Send(new CreateVacancyCommand(vacancy), ct);
        return CreatedAtAction(nameof(Get), new { id }, new { id });
    }

    [Authorize(Roles = Roles.Industry)]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, VacancyInput vacancy, CancellationToken ct)
    {
        await Mediator.Send(new UpdateVacancyCommand(id, vacancy), ct);
        return NoContent();
    }

    [Authorize(Roles = Roles.Admin)]
    [HttpPost("{id:int}/review")]
    public async Task<IActionResult> Review(int id, ReviewRequest body, CancellationToken ct)
    {
        await Mediator.Send(new ReviewVacancyCommand(id, body.Decision, body.Note), ct);
        return NoContent();
    }
}

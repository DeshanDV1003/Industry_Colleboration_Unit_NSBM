using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSBM.ICU.Application.Features.Interviews.Dtos;
using NSBM.ICU.Application.Features.Interviews.Queries.GetStudentInterviewReport;
using NSBM.ICU.Application.Features.Lecturing.Commands.AddSupportNote;
using NSBM.ICU.Application.Features.Lecturing.Commands.AssignTask;
using NSBM.ICU.Application.Features.Lecturing.Dtos;
using NSBM.ICU.Application.Features.Lecturing.Queries.GetAssignableTasks;
using NSBM.ICU.Application.Features.Lecturing.Queries.GetStudentDirectory;
using NSBM.ICU.Application.Features.Lecturing.Queries.GetStudentInsights;
using NSBM.ICU.Domain.Constants;

namespace NSBM.ICU.API.Controllers;

/// <summary>Lecturers monitoring and supporting students.</summary>
[Authorize(Roles = Roles.Lecturer)]
[Route(ApiRoutes.V1 + "/lecturer")]
public sealed class LecturerController : ApiControllerBase
{
    public sealed record NoteRequest(string Text);
    public sealed record AssignRequest(int TaskId, string? Note);

    [HttpGet("students")]
    public Task<IReadOnlyList<StudentDirectoryItemDto>> Directory([FromQuery] string? search, [FromQuery] bool needsFollowUp, CancellationToken ct) =>
        Mediator.Send(new GetStudentDirectoryQuery(search, needsFollowUp), ct);

    [HttpGet("students/{id:int}")]
    public Task<StudentInsightsDto> Insights(int id, CancellationToken ct) => Mediator.Send(new GetStudentInsightsQuery(id), ct);

    [HttpGet("students/{id:int}/interview-report")]
    public Task<InterviewReportDto> InterviewReport(int id, CancellationToken ct) => Mediator.Send(new GetStudentInterviewReportQuery(id), ct);

    [HttpPost("students/{id:int}/notes")]
    public async Task<IActionResult> AddNote(int id, NoteRequest body, CancellationToken ct) =>
        Ok(new { id = await Mediator.Send(new AddSupportNoteCommand(id, body.Text), ct) });

    [HttpPost("students/{id:int}/assignments")]
    public async Task<IActionResult> Assign(int id, AssignRequest body, CancellationToken ct) =>
        Ok(new { id = await Mediator.Send(new AssignTaskCommand(id, body.TaskId, body.Note), ct) });

    [HttpGet("assignable-tasks")]
    public Task<IReadOnlyList<AssignableTaskDto>> AssignableTasks(CancellationToken ct) => Mediator.Send(new GetAssignableTasksQuery(), ct);
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSBM.ICU.Application.Features.Users.Commands.CreateUser;
using NSBM.ICU.Application.Features.Users.Commands.ImportStudents;
using NSBM.ICU.Application.Features.Users.Commands.ResetUserPassword;
using NSBM.ICU.Application.Features.Users.Commands.SetUserActive;
using NSBM.ICU.Application.Features.Users.Commands.UpdateUser;
using NSBM.ICU.Application.Features.Users.Dtos;
using NSBM.ICU.Application.Features.Users.Models;
using NSBM.ICU.Application.Features.Users.Queries.GetUser;
using NSBM.ICU.Application.Features.Users.Queries.GetUserOptions;
using NSBM.ICU.Application.Features.Users.Queries.GetUsers;
using NSBM.ICU.Domain.Constants;

namespace NSBM.ICU.API.Controllers;

/// <summary>Accounts for every role, managed by the Unit admin.</summary>
[Authorize(Roles = Roles.Admin)]
[Route(ApiRoutes.V1 + "/users")]
public sealed class UsersController : ApiControllerBase
{
    public sealed record UpdateUserBody(string Role, UserProfileInput Profile);
    public sealed record ResetPasswordBody(string NewPassword);

    [HttpGet]
    public Task<IReadOnlyList<UserDto>> List([FromQuery] string? role, [FromQuery] string? search, CancellationToken ct) =>
        Mediator.Send(new GetUsersQuery(role, search), ct);

    [HttpGet("options")]
    public Task<UserOptionsDto> Options(CancellationToken ct) => Mediator.Send(new GetUserOptionsQuery(), ct);

    [HttpGet("{userId}")]
    public Task<UserDetailDto> Get(string userId, CancellationToken ct) => Mediator.Send(new GetUserQuery(userId), ct);

    [HttpPost]
    public async Task<IActionResult> Create(CreateUserCommand body, CancellationToken ct) =>
        Ok(new { userId = await Mediator.Send(body, ct) });

    [HttpPut("{userId}")]
    public async Task<IActionResult> Update(string userId, UpdateUserBody body, CancellationToken ct)
    {
        await Mediator.Send(new UpdateUserCommand(userId, body.Role, body.Profile), ct);
        return NoContent();
    }

    [HttpPost("{userId}/deactivate")]
    public async Task<IActionResult> Deactivate(string userId, CancellationToken ct)
    {
        await Mediator.Send(new SetUserActiveCommand(userId, false), ct);
        return NoContent();
    }

    [HttpPost("{userId}/activate")]
    public async Task<IActionResult> Activate(string userId, CancellationToken ct)
    {
        await Mediator.Send(new SetUserActiveCommand(userId, true), ct);
        return NoContent();
    }

    [HttpPost("{userId}/reset-password")]
    public async Task<IActionResult> ResetPassword(string userId, ResetPasswordBody body, CancellationToken ct)
    {
        await Mediator.Send(new ResetUserPasswordCommand(userId, body.NewPassword), ct);
        return NoContent();
    }

    [HttpPost("import-students")]
    public Task<StudentImportResultDto> ImportStudents(ImportStudentsCommand body, CancellationToken ct) => Mediator.Send(body, ct);
}

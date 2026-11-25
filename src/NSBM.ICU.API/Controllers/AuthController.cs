using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSBM.ICU.Application.Features.Auth.Commands.ChangePassword;
using NSBM.ICU.Application.Features.Auth.Commands.DemoLogin;
using NSBM.ICU.Application.Features.Auth.Commands.Login;
using NSBM.ICU.Application.Features.Auth.Dtos;
using NSBM.ICU.Application.Features.Auth.Queries.GetCurrentUser;

namespace NSBM.ICU.API.Controllers;

[Route(ApiRoutes.V1 + "/auth")]
public sealed class AuthController : ApiControllerBase
{
    [AllowAnonymous]
    [HttpPost("login")]
    public Task<AuthResponseDto> Login(LoginCommand command, CancellationToken ct) =>
        Mediator.Send(command, ct);

    [AllowAnonymous]
    [HttpPost("demo-login")]
    public Task<AuthResponseDto> DemoLogin(DemoLoginCommand command, CancellationToken ct) =>
        Mediator.Send(command, ct);

    [Authorize]
    [HttpGet("me")]
    public Task<CurrentUserDto> Me(CancellationToken ct) =>
        Mediator.Send(new GetCurrentUserQuery(), ct);

    [Authorize]
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword(ChangePasswordCommand command, CancellationToken ct)
    {
        await Mediator.Send(command, ct);
        return NoContent();
    }
}

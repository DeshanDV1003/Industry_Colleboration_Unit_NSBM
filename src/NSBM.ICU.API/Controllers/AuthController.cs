using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using NSBM.ICU.API.Extensions;
using NSBM.ICU.Application.Features.Auth.Commands.ChangePassword;
using NSBM.ICU.Application.Features.Auth.Commands.DemoLogin;
using NSBM.ICU.Application.Features.Auth.Commands.ForgotPassword;
using NSBM.ICU.Application.Features.Auth.Commands.Login;
using NSBM.ICU.Application.Features.Auth.Commands.ResetForgottenPassword;
using NSBM.ICU.Application.Features.Auth.Dtos;
using NSBM.ICU.Application.Features.Auth.Queries.GetCurrentUser;
using NSBM.ICU.Application.Features.Auth.Queries.GetPasswordResetAvailability;

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

    [AllowAnonymous]
    [HttpGet("password-reset")]
    public Task<PasswordResetAvailabilityDto> PasswordResetAvailability(CancellationToken ct) =>
        Mediator.Send(new GetPasswordResetAvailabilityQuery(), ct);

    /// <summary>Always 202 for a valid address, whether or not it has an account, so the answer reveals nothing.</summary>
    [AllowAnonymous]
    [EnableRateLimiting(PresentationServiceExtensions.PasswordResetRateLimit)]
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordCommand command, CancellationToken ct)
    {
        await Mediator.Send(command, ct);
        return Accepted();
    }

    [AllowAnonymous]
    [EnableRateLimiting(PresentationServiceExtensions.PasswordResetRateLimit)]
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(ResetForgottenPasswordCommand command, CancellationToken ct)
    {
        await Mediator.Send(command, ct);
        return NoContent();
    }

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

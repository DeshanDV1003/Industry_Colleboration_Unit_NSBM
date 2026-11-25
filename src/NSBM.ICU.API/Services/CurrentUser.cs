using System.Security.Claims;
using NSBM.ICU.Application.Common.Interfaces;

namespace NSBM.ICU.API.Services;

public sealed class CurrentUser(IHttpContextAccessor accessor) : ICurrentUser
{
    private ClaimsPrincipal? Principal => accessor.HttpContext?.User;

    public string? UserId => Principal?.FindFirstValue(ClaimTypes.NameIdentifier);
    public string? Role => Principal?.FindFirstValue(ClaimTypes.Role);
}

using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace NSBM.ICU.API.Controllers;

/// <summary>Base for every API controller. Controllers only translate HTTP to MediatR requests.</summary>
/// <remarks>Each controller declares its own <c>[Route]</c> starting with <see cref="ApiRoutes.V1"/>.</remarks>
[ApiController]
[Produces("application/json")]
public abstract class ApiControllerBase : ControllerBase
{
    private ISender? _mediator;

    protected ISender Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<ISender>();
}

public static class ApiRoutes
{
    public const string V1 = "api/v1";
}

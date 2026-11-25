using Microsoft.AspNetCore.Mvc;
using NSBM.ICU.Application.Common.Exceptions;
using NSBM.ICU.Domain.Common;

namespace NSBM.ICU.API.Middleware;

/// <summary>Translates application exceptions into RFC 7807 problem responses.</summary>
public sealed class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            var problem = ToProblem(exception);
            if (problem.Status == StatusCodes.Status500InternalServerError)
                logger.LogError(exception, "Unhandled exception for {Method} {Path}", context.Request.Method, context.Request.Path);

            context.Response.StatusCode = problem.Status!.Value;
            await context.Response.WriteAsJsonAsync(problem, problem.GetType(), options: null, contentType: "application/problem+json");
        }
    }

    private static ProblemDetails ToProblem(Exception exception) => exception switch
    {
        ValidationException e => new ValidationProblemDetails(e.Errors.ToDictionary(kv => kv.Key, kv => kv.Value))
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Validation failed",
            Detail = e.Message,
        },
        BusinessRuleException e => Problem(StatusCodes.Status400BadRequest, "Request could not be completed", e.Message),
        DomainRuleException e => Problem(StatusCodes.Status400BadRequest, "Request could not be completed", e.Message),
        AiServiceUnavailableException e => Problem(StatusCodes.Status503ServiceUnavailable, "AI service unavailable", e.Message),
        AuthenticationFailedException e => Problem(StatusCodes.Status401Unauthorized, "Authentication failed", e.Message),
        ForbiddenAccessException e => Problem(StatusCodes.Status403Forbidden, "Forbidden", e.Message),
        NotFoundException e => Problem(StatusCodes.Status404NotFound, "Not found", e.Message),
        _ => Problem(StatusCodes.Status500InternalServerError, "Server error", "An unexpected error occurred."),
    };

    private static ProblemDetails Problem(int status, string title, string detail) =>
        new() { Status = status, Title = title, Detail = detail };
}

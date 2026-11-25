namespace NSBM.ICU.API.Middleware;

/// <summary>Conservative headers for an API that only returns JSON and files: no sniffing, no framing, no referrer leaks.</summary>
public sealed class SecurityHeadersMiddleware(RequestDelegate next)
{
    public Task InvokeAsync(HttpContext context)
    {
        var headers = context.Response.Headers;
        headers.XContentTypeOptions = "nosniff";
        headers.XFrameOptions = "DENY";
        headers["Referrer-Policy"] = "no-referrer";
        headers["Cross-Origin-Resource-Policy"] = "same-site";
        return next(context);
    }
}

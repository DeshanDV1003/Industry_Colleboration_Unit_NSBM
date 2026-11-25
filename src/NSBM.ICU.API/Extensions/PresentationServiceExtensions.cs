using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Net;
using Microsoft.AspNetCore.HttpOverrides;
using NSBM.ICU.API.Serialization;
using NSBM.ICU.API.Services;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Infrastructure.Identity;

namespace NSBM.ICU.API.Extensions;

public static class PresentationServiceExtensions
{
    public const string WebClientCorsPolicy = "WebClient";
    public const string PublicFormsRateLimit = "PublicForms";
    public const string ReadyTag = "ready";

    public static IServiceCollection AddPresentation(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUser, CurrentUser>();
        services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                options.JsonSerializerOptions.Converters.Add(new UtcDateTimeConverter());
            });

        services.AddJwtAuthentication(configuration);
        services.AddAuthorization();
        services.AddWebClientCors(configuration);
        services.AddSwagger();
        services.AddPublicFormRateLimit();
        services.AddProxySupport(configuration);
        services.AddHealthChecks().AddCheck<DatabaseHealthCheck>("database", tags: [ReadyTag]);

        return services;
    }

    private static void AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var jwt = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>()
                  ?? throw new InvalidOperationException("JWT settings are not configured.");

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwt.Issuer,
                ValidAudience = jwt.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Secret)),
                ClockSkew = TimeSpan.FromMinutes(1),
            };

                // A deactivated account loses access at once, not when its token expires.
                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = async context =>
                    {
                        var userId = context.Principal?.FindFirstValue(ClaimTypes.NameIdentifier);
                        var identity = context.HttpContext.RequestServices.GetRequiredService<IIdentityService>();
                        if (userId is null || !await identity.IsActiveAsync(userId, context.HttpContext.RequestAborted))
                            context.Fail("This account is no longer active.");
                    },
                };
            });
    }

    private static void AddWebClientCors(this IServiceCollection services, IConfiguration configuration)
    {
        var origins = configuration.GetSection("AllowedOrigins").Get<string[]>() ?? [];

        services.AddCors(options => options.AddPolicy(WebClientCorsPolicy, policy => policy
            .WithOrigins(origins)
            .AllowAnyHeader()
            .AllowAnyMethod()));
    }

    /// <summary>Public forms (no sign-in) accept a few submissions per minute from each address, to slow down spam.</summary>
    private static void AddPublicFormRateLimit(this IServiceCollection services) =>
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            options.AddPolicy(PublicFormsRateLimit, context => RateLimitPartition.GetFixedWindowLimiter(
                context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                _ => new FixedWindowRateLimiterOptions { PermitLimit = 5, Window = TimeSpan.FromMinutes(1), QueueLimit = 0 }));
        });

    private static void AddSwagger(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "NSBM Industry Collaboration Unit API", Version = "v1" });

            var scheme = new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" },
            };
            options.AddSecurityDefinition("Bearer", scheme);
            options.AddSecurityRequirement(new OpenApiSecurityRequirement { [scheme] = [] });
        });
    }

    /// <summary>
    /// Behind a reverse proxy (nginx, a load balancer) the client's address arrives in X-Forwarded-For. Only the
    /// listed proxies are trusted, so the public-form rate limit counts real clients, not the proxy.
    /// </summary>
    private static void AddProxySupport(this IServiceCollection services, IConfiguration configuration)
    {
        var section = configuration.GetSection("ForwardedHeaders");
        services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            foreach (var proxy in section.GetSection("KnownProxies").Get<string[]>() ?? [])
                options.KnownProxies.Add(IPAddress.Parse(proxy));
            foreach (var network in section.GetSection("KnownNetworks").Get<string[]>() ?? [])
            {
                var parts = network.Split('/');
                options.KnownNetworks.Add(new Microsoft.AspNetCore.HttpOverrides.IPNetwork(IPAddress.Parse(parts[0]), int.Parse(parts[1])));
            }
        });
    }
}

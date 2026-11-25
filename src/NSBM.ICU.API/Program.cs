using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using NSBM.ICU.API.Extensions;
using NSBM.ICU.API.Middleware;
using NSBM.ICU.Application;
using NSBM.ICU.Infrastructure;
using NSBM.ICU.Infrastructure.Persistence.Seed;

var builder = WebApplication.CreateBuilder(args);
builder.EnsureSafeSettings();

builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration)
    .AddPresentation(builder.Configuration);

var app = builder.Build();

await using (var scope = app.Services.CreateAsyncScope())
{
    await scope.ServiceProvider.GetRequiredService<DatabaseSeeder>().InitialiseAsync();
}

if (app.Configuration.GetValue<bool>("ForwardedHeaders:Enabled"))
    app.UseForwardedHeaders();

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<SecurityHeadersMiddleware>();

if (!app.Environment.IsDevelopment())
    app.UseHsts();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(PresentationServiceExtensions.WebClientCorsPolicy);
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Live: the process answers. Ready: it can also reach the database.
app.MapHealthChecks("/health/live", new HealthCheckOptions { Predicate = _ => false });
app.MapHealthChecks("/health/ready", new HealthCheckOptions { Predicate = c => c.Tags.Contains(PresentationServiceExtensions.ReadyTag) });

app.Run();

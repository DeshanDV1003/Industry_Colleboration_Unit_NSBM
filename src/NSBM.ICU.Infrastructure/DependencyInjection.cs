using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NSBM.ICU.Application.Common.Auditing;
using NSBM.ICU.Application.Common.Interfaces;
using NSBM.ICU.Infrastructure.Ai;
using NSBM.ICU.Infrastructure.Identity;
using NSBM.ICU.Infrastructure.Persistence;
using NSBM.ICU.Infrastructure.Persistence.Interceptors;
using NSBM.ICU.Infrastructure.Persistence.Seed;
using NSBM.ICU.Infrastructure.Reports;
using NSBM.ICU.Infrastructure.Services;

namespace NSBM.ICU.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddPersistence(configuration);
        services.AddIdentityServices(configuration);
        services.AddSingleton<IDateTime, DateTimeService>();
        services.AddSingleton<IReportFileWriter, ExcelReportWriter>();
        services.AddAiServices(configuration);
        return services;
    }

    /// <summary>The key comes from user-secrets ("Anthropic:ApiKey") or the ANTHROPIC_API_KEY environment variable.</summary>
    private static void AddAiServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AnthropicOptions>(configuration.GetSection(AnthropicOptions.SectionName));
        services.PostConfigure<AnthropicOptions>(o => o.ApiKey ??= configuration["ANTHROPIC_API_KEY"]);
        services.AddSingleton<IAiInterviewer, ClaudeAiInterviewer>();
    }

    private static void AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured (ConnectionStrings__DefaultConnection).");

        services.AddScoped<AuditableEntityInterceptor>();
        services.AddDbContext<ApplicationDbContext>((sp, options) => options
            .UseSqlServer(connectionString, sql => sql
                .MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)
                // Loading several child collections in one JOIN multiplies rows; split them into separate queries.
                .UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery))
            .AddInterceptors(sp.GetRequiredService<AuditableEntityInterceptor>()));

        services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<ApplicationDbContext>());
        services.AddScoped<CatalogueSeeder>();
        services.AddScoped<PeopleSeeder>();
        services.AddScoped<TaskSeeder>();
        services.AddScoped<OpportunitySeeder>();
        services.AddScoped<InterviewSeeder>();
        services.AddScoped<SimulationSeeder>();
        services.AddScoped<EventSeeder>();
        services.AddScoped<AchievementSeeder>();
        services.AddScoped<CalendarSeeder>();
        services.AddScoped<VenueSeeder>();
        services.AddScoped<BootstrapAdminSeeder>();
        services.AddScoped<DatabaseSeeder>();
        services.AddScoped<IDemoDataService, DemoDataService>();
        services.AddScoped<IAuditTrail, AuditTrail>();
    }

    private static void AddIdentityServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        services.Configure<DemoOptions>(configuration.GetSection(DemoOptions.SectionName));
        services.Configure<BootstrapOptions>(configuration.GetSection(BootstrapOptions.SectionName));

        services.AddIdentityCore<ApplicationUser>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = true;
                options.Password.RequireNonAlphanumeric = false;
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            // Needed for admin password resets (a reset token is generated and used at once).
            .AddDefaultTokenProviders();

        services.AddScoped<IIdentityService, IdentityService>();
        services.AddScoped<ITokenService, TokenService>();
    }
}

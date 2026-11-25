using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using NSBM.ICU.Application.Common.Behaviours;
using NSBM.ICU.Application.Features.AiInterviews;
using NSBM.ICU.Application.Features.Calendar;

namespace NSBM.ICU.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        services.AddValidatorsFromAssembly(assembly);
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(assembly);
            cfg.AddOpenBehavior(typeof(ValidationBehaviour<,>));
        });

        services.AddScoped<CalendarEntryReader>();
        services.AddScoped<AiInterviewConductor>();

        return services;
    }
}

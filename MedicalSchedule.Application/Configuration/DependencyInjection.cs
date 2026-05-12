using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace MedicalSchedule.Application.Configuration;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<ApplicationAssemblyMarker>(lifetime: ServiceLifetime.Scoped);

        return services;
    }
}

public sealed class ApplicationAssemblyMarker;

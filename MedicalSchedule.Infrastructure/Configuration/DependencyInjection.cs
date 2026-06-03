using MedicalSchedule.Application.Abstractions;
using MedicalSchedule.Application.Configuration;
using MedicalSchedule.Domain.Abstractions;
using MedicalSchedule.Infrastructure.Auth;
using MedicalSchedule.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MedicalSchedule.Infrastructure.Configuration;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<AppDbContext>());
        services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();

        // Auto-register all Command/Query handlers (AsSelf, Scoped)
        services.Scan(scan => scan
            .FromAssemblies(typeof(ApplicationAssemblyMarker).Assembly)
            .AddClasses(c => c.Where(t =>
                t.Name.EndsWith("CommandHandler") || t.Name.EndsWith("Query")))
            .AsSelf()
            .WithScopedLifetime());

        // Auto-register all domain event handlers
        services.Scan(scan => scan
            .FromAssemblies(typeof(ApplicationAssemblyMarker).Assembly)
            .AddClasses(c => c.AssignableTo(typeof(IDomainEventHandler<>)))
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        return services;
    }
}

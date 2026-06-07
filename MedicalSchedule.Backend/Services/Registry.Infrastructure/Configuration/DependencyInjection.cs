using BuildingBlocks.Messaging.Extensions;
using BuildingBlocks.Persistence.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Registry.Features.Abstractions;
using Registry.Infrastructure.Persistence;
using Registry.Infrastructure.Persistence.Repositories;

namespace Registry.Infrastructure.Configuration;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        string connectionString,
        string rabbitMqConnectionString)
    {
        services.AddPersistence<RegistryDbContext>(connectionString);
        services.AddMessaging(rabbitMqConnectionString);
        services.AddOutboxWorker<RegistryDbContext>();

        services.AddScoped<IPetRepository, PetRepository>();
        services.AddScoped<IOwnerRepository, OwnerRepository>();
        services.AddScoped<IVetRepository, VetRepository>();

        return services;
    }

}

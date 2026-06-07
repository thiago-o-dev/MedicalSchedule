using BuildingBlocks.Messaging.Extensions;
using BuildingBlocks.Persistence;
using BuildingBlocks.Persistence.Abstractions;
using BuildingBlocks.Persistence.EntityFramework;
using BuildingBlocks.Persistence.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Registry.Features.Shared;
using Registry.Infrastructure.Persistence;
using Registry.Infrastructure.Persistence.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace Registry.Infrastructure;

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
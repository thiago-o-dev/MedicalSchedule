using Caching.Redis;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Caching.Extensions;

public static class CachingExtensions
{
    public static IServiceCollection AddSlotLockService(this IServiceCollection services, string connectionString)
    {
        services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(connectionString));
        services.AddSingleton<ISlotLockService, RedisSlotLockService>();
        return services;
    }
}

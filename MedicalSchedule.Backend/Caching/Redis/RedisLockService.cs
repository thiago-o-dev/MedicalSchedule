using StackExchange.Redis;

namespace Caching.Redis;

internal sealed class RedisSlotLockService(IConnectionMultiplexer redis) : ISlotLockService
{
    private static readonly TimeSpan LockTtl = TimeSpan.FromSeconds(30);

    public async Task<bool> TryAcquireSlotLockAsync(Guid vetId, DateTime scheduledAt, CancellationToken cancellationToken = default)
    {
        var db = redis.GetDatabase();
        var key = $"slot-lock:{vetId}:{scheduledAt:yyyyMMddHHmm}";
        return await db.StringSetAsync(key, "1", LockTtl, When.NotExists);
    }
}

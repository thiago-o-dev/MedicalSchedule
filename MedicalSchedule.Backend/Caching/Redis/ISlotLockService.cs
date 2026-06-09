namespace Caching.Redis;

public interface ISlotLockService
{
    Task<bool> TryAcquireSlotLockAsync(Guid vetId, DateTime scheduledAt, CancellationToken cancellationToken = default);
}

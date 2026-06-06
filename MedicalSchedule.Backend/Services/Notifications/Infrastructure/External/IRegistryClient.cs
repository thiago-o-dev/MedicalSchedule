namespace Notifications.Infrastructure.External;

public record OwnerContact(string Name, string Email);

public interface IRegistryClient
{
    Task<OwnerContact?> GetOwnerByPetIdAsync(Guid petId, CancellationToken cancellationToken = default);
}

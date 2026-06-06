using Registry.Domain.Entities;

namespace Registry.Features.Shared;

public interface IOwnerRepository
{
    Task AddAsync(Owner owner, CancellationToken cancellationToken = default);
    Task<Owner?> GetOwnerByPetIdAsync(Guid petId, CancellationToken cancellationToken = default);
}

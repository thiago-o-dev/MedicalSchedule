using Registry.Domain.Entities;

namespace Registry.Features.Abstractions;

public interface IOwnerRepository
{
    Task AddAsync(Owner owner, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Owner>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Owner?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Owner?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<Owner?> GetOwnerByPetIdAsync(Guid petId, CancellationToken cancellationToken = default);
}

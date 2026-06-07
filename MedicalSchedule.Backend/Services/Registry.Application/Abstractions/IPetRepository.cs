using Registry.Domain.Entities;

namespace Registry.Features.Abstractions;

public interface IPetRepository
{
    Task AddAsync(Pet pet, CancellationToken cancellationToken = default);
    Task UpdateAsync(Pet pet, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Pet>> GetAllAsync(Guid? ownerId = null, CancellationToken cancellationToken = default);
    Task<Pet?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}

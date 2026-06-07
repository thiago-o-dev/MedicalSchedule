using Registry.Domain.Entities;

namespace Registry.Features.Shared;

public interface IVetRepository
{
    Task AddAsync(Vet vet, CancellationToken cancellationToken = default);
    Task UpdateAsync(Vet vet, CancellationToken cancellationToken = default);
    Task<Vet?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}

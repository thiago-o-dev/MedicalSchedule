using Registry.Domain.Entities;

namespace Registry.Features.Abstractions;

public interface IVetRepository
{
    Task AddAsync(Vet vet, CancellationToken cancellationToken = default);
    Task UpdateAsync(Vet vet, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Vet>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Vet?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}

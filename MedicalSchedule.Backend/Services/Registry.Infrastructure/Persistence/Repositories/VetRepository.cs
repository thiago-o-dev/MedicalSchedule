using Microsoft.EntityFrameworkCore;
using Registry.Domain.Entities;
using Registry.Features.Abstractions;

namespace Registry.Infrastructure.Persistence.Repositories;

public sealed class VetRepository(RegistryDbContext dbContext) : IVetRepository
{
    public async Task AddAsync(Vet vet, CancellationToken cancellationToken = default)
        => await dbContext.Vets.AddAsync(vet, cancellationToken);

    public Task UpdateAsync(Vet vet, CancellationToken cancellationToken = default)
    {
        dbContext.Vets.Update(vet);
        return Task.CompletedTask;
    }

    public Task<Vet?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => dbContext.Vets.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
}

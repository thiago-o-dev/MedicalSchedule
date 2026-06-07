using Microsoft.EntityFrameworkCore;
using Registry.Domain.Entities;
using Registry.Features.Abstractions;

namespace Registry.Infrastructure.Persistence.Repositories;

public sealed class PetRepository(RegistryDbContext dbContext) : IPetRepository
{
    public async Task AddAsync(Pet pet, CancellationToken cancellationToken = default)
        => await dbContext.Pets.AddAsync(pet, cancellationToken);

    public Task UpdateAsync(Pet pet, CancellationToken cancellationToken = default)
    {
        dbContext.Pets.Update(pet);
        return Task.CompletedTask;
    }

    public Task<Pet?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => dbContext.Pets
            .Include(x => x.Ownerships)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
}

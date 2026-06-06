using Microsoft.EntityFrameworkCore;
using Registry.Domain.Entities;
using Registry.Features.Shared;

namespace Registry.Infrastructure.Persistence.Repositories;

public sealed class PetRepository : IPetRepository
{
    private readonly RegistryDbContext _dbContext;

    public PetRepository(
        RegistryDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(Pet pet)
    {
        await _dbContext.Pets.AddAsync(pet);
    }

    public async Task UpdateAsync(Pet pet)
    {
        _dbContext.Pets.Update(pet);

        await Task.CompletedTask;
    }

    public async Task<Pet> GetByIdAsync(Guid id)
    {
        var pet = await _dbContext.Pets
            .Include(x => x.Ownerships)
            .FirstOrDefaultAsync(x => x.Id == id);

        return pet!;
    }
}
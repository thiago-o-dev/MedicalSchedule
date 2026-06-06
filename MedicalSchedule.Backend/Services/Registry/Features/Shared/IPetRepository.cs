using Registry.Domain.Entities;

namespace Registry.Features.Shared;

public interface IPetRepository
{
    Task AddAsync(Pet pet);

    Task UpdateAsync(Pet pet);

    Task<Pet> GetByIdAsync(Guid id);
}
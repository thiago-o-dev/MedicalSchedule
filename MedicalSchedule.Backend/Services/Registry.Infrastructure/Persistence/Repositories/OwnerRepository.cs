using Microsoft.EntityFrameworkCore;
using Registry.Domain.Entities;
using Registry.Features.Abstractions;

namespace Registry.Infrastructure.Persistence.Repositories;

public sealed class OwnerRepository(RegistryDbContext dbContext) : IOwnerRepository
{
    public async Task AddAsync(Owner owner, CancellationToken cancellationToken = default)
        => await dbContext.Owners.AddAsync(owner, cancellationToken);

    public async Task<IReadOnlyList<Owner>> GetAllAsync(CancellationToken cancellationToken = default)
        => await dbContext.Owners
            .OrderBy(o => o.Name)
            .ToListAsync(cancellationToken);

    public Task<Owner?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => dbContext.Owners.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task<Owner?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        => dbContext.Owners.FirstOrDefaultAsync(x => x.Email.ToLower() == email.ToLower(), cancellationToken);

    public Task<Owner?> GetByCpfAsync(string cpf, CancellationToken cancellationToken = default)
        => dbContext.Owners.FirstOrDefaultAsync(x => x.Cpf == cpf, cancellationToken);

    public Task<Owner?> GetOwnerByPetIdAsync(Guid petId, CancellationToken cancellationToken = default)
        => dbContext.Owners
            .Join(
                dbContext.Set<PetOwnership>(),
                owner => owner.Id,
                ownership => ownership.OwnerId,
                (owner, ownership) => new { owner, ownership })
            .Where(x => x.ownership.PetId == petId)
            .OrderByDescending(x => x.ownership.IsPrimaryOwner)
            .Select(x => x.owner)
            .FirstOrDefaultAsync(cancellationToken);
}

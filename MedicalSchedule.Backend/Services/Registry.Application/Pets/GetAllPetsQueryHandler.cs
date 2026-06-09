using Registry.Features.Abstractions;
using SharedKernel.Abstractions;

namespace Registry.Features.Pets;

public sealed class GetAllPetsQueryHandler(IPetRepository petRepository)
    : IQueryHandler<GetAllPetsQuery, IReadOnlyList<PetResponse>>
{
    public async Task<IReadOnlyList<PetResponse>> HandleAsync(
        GetAllPetsQuery query,
        CancellationToken cancellationToken = default)
    {
        var pets = await petRepository.GetAllAsync(query.OwnerId, cancellationToken);

        return pets
            .Select(p => new PetResponse(
                p.Id,
                p.Name,
                p.Species,
                p.Breed,
                p.BirthDate,
                p.IsActive,
                p.DeletionStatus,
                p.DeletionRejectionReason,
                p.Ownerships
                    .Select(o => new PetOwnershipResponse(o.OwnerId, o.IsPrimaryOwner))
                    .ToList()))
            .ToList();
    }
}

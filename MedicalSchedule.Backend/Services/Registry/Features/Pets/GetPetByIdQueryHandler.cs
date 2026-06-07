using Registry.Features.Shared;
using SharedKernel.Abstractions;
using SharedKernel.Exceptions;

namespace Registry.Features.Pets;

public sealed class GetPetByIdQueryHandler(IPetRepository petRepository)
    : IQueryHandler<GetPetByIdQuery, PetResponse>
{
    public async Task<PetResponse> HandleAsync(
        GetPetByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var pet = await petRepository.GetByIdAsync(query.PetId, cancellationToken)
            ?? throw new NotFoundException($"Pet '{query.PetId}' not found.");

        return new PetResponse(pet.Id, pet.Name, pet.Species, pet.Breed, pet.BirthDate, pet.IsActive);
    }
}

using Registry.Features.Abstractions;
using SharedKernel.Abstractions;

namespace Registry.Features.Pets;

public sealed class GetPetOwnerQueryHandler(IOwnerRepository ownerRepository)
    : IQueryHandler<GetPetOwnerQuery, OwnerContactResponse?>
{
    public async Task<OwnerContactResponse?> HandleAsync(
        GetPetOwnerQuery query,
        CancellationToken cancellationToken = default)
    {
        var owner = await ownerRepository.GetOwnerByPetIdAsync(query.PetId, cancellationToken);

        return owner is null ? null : new OwnerContactResponse(owner.Name, owner.Email);
    }
}

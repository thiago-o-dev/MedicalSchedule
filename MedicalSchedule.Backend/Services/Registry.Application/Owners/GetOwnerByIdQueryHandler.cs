using Registry.Features.Abstractions;
using SharedKernel.Abstractions;
using SharedKernel.Exceptions;

namespace Registry.Features.Owners;

public sealed class GetOwnerByIdQueryHandler(IOwnerRepository ownerRepository)
    : IQueryHandler<GetOwnerByIdQuery, OwnerResponse>
{
    public async Task<OwnerResponse> HandleAsync(
        GetOwnerByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var owner = await ownerRepository.GetByIdAsync(query.OwnerId, cancellationToken)
            ?? throw new NotFoundException($"Owner '{query.OwnerId}' not found.");

        return new OwnerResponse(owner.Id, owner.Name, owner.Cpf, owner.Phone, owner.Email, owner.IsActive);
    }
}

using Registry.Features.Abstractions;
using SharedKernel.Abstractions;
using SharedKernel.Exceptions;

namespace Registry.Features.Owners;

public sealed class GetOwnerByEmailQueryHandler(IOwnerRepository ownerRepository)
    : IQueryHandler<GetOwnerByEmailQuery, OwnerResponse>
{
    public async Task<OwnerResponse> HandleAsync(
        GetOwnerByEmailQuery query,
        CancellationToken cancellationToken = default)
    {
        var owner = await ownerRepository.GetByEmailAsync(query.Email, cancellationToken)
            ?? throw new NotFoundException($"Owner with email '{query.Email}' not found.");

        return new OwnerResponse(owner.Id, owner.Name, owner.Cpf, owner.Phone, owner.Email, owner.IsActive);
    }
}

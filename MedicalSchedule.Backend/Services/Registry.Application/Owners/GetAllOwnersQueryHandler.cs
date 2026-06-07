using Registry.Features.Abstractions;
using SharedKernel.Abstractions;

namespace Registry.Features.Owners;

public sealed class GetAllOwnersQueryHandler(IOwnerRepository ownerRepository)
    : IQueryHandler<GetAllOwnersQuery, IReadOnlyList<OwnerResponse>>
{
    public async Task<IReadOnlyList<OwnerResponse>> HandleAsync(
        GetAllOwnersQuery query,
        CancellationToken cancellationToken = default)
    {
        var owners = await ownerRepository.GetAllAsync(cancellationToken);

        return owners
            .Select(o => new OwnerResponse(o.Id, o.Name, o.Cpf, o.Phone, o.Email, o.IsActive))
            .ToList();
    }
}

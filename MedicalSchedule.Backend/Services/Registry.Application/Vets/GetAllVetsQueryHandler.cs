using Registry.Features.Abstractions;
using SharedKernel.Abstractions;

namespace Registry.Features.Vets;

public sealed class GetAllVetsQueryHandler(IVetRepository vetRepository)
    : IQueryHandler<GetAllVetsQuery, IReadOnlyList<VetResponse>>
{
    public async Task<IReadOnlyList<VetResponse>> HandleAsync(
        GetAllVetsQuery query,
        CancellationToken cancellationToken = default)
    {
        var vets = await vetRepository.GetAllAsync(cancellationToken);

        return vets
            .Select(v => new VetResponse(v.Id, v.Name, v.Crm, v.Specialty, v.Email, v.IsActive))
            .ToList();
    }
}

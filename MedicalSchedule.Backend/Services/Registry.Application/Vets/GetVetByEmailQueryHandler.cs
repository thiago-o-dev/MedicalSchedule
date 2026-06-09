using Registry.Features.Abstractions;
using SharedKernel.Abstractions;
using SharedKernel.Exceptions;

namespace Registry.Features.Vets;

public sealed class GetVetByEmailQueryHandler(IVetRepository vetRepository)
    : IQueryHandler<GetVetByEmailQuery, VetResponse>
{
    public async Task<VetResponse> HandleAsync(
        GetVetByEmailQuery query,
        CancellationToken cancellationToken = default)
    {
        var vet = await vetRepository.GetByEmailAsync(query.Email, cancellationToken)
            ?? throw new NotFoundException($"Vet with email '{query.Email}' not found.");

        return new VetResponse(vet.Id, vet.Name, vet.Crm, vet.Specialty, vet.Email, vet.IsActive);
    }
}

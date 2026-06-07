using Registry.Features.Abstractions;
using SharedKernel.Abstractions;
using SharedKernel.Exceptions;

namespace Registry.Features.Vets;

public sealed class GetVetByIdQueryHandler(IVetRepository vetRepository)
    : IQueryHandler<GetVetByIdQuery, VetResponse>
{
    public async Task<VetResponse> HandleAsync(
        GetVetByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var vet = await vetRepository.GetByIdAsync(query.VetId, cancellationToken)
            ?? throw new NotFoundException($"Vet '{query.VetId}' not found.");

        return new VetResponse(vet.Id, vet.Name, vet.Crm, vet.Specialty, vet.Email, vet.IsActive);
    }
}

using BuildingBlocks.Persistence.Abstractions;
using Registry.Features.Abstractions;
using SharedKernel.Abstractions;
using SharedKernel.Exceptions;

namespace Registry.Features.Vets;

public sealed class DeactivateVetCommandHandler(
    IVetRepository vetRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<DeactivateVetCommand>
{
    public async Task HandleAsync(DeactivateVetCommand command, CancellationToken cancellationToken = default)
    {
        var vet = await vetRepository.GetByIdAsync(command.VetId, cancellationToken)
            ?? throw new NotFoundException($"Vet '{command.VetId}' not found.");

        vet.Deactivate();

        await vetRepository.UpdateAsync(vet, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

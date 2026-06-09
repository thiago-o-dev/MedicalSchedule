using BuildingBlocks.Persistence.Abstractions;
using Microsoft.Extensions.Logging;
using Registry.Application.Events;
using Registry.Features.Abstractions;
using SharedKernel.Abstractions;

namespace Registry.Features.Pets;

public sealed class PetDeletionApprovedEventHandler(
    IPetRepository petRepository,
    IUnitOfWork unitOfWork,
    ILogger<PetDeletionApprovedEventHandler> logger) : IDomainEventHandler<PetDeletionApprovedEvent>
{
    public async Task HandleAsync(PetDeletionApprovedEvent @event, CancellationToken cancellationToken = default)
    {
        var pet = await petRepository.GetByIdAsync(@event.PetId, cancellationToken);
        if (pet is null)
        {
            logger.LogWarning("PetDeletionApprovedEvent ignored: pet {PetId} not found.", @event.PetId);
            return;
        }

        pet.ConfirmDeletion();

        await petRepository.UpdateAsync(pet, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Pet {PetId} soft-deleted after saga approval.", @event.PetId);
    }
}

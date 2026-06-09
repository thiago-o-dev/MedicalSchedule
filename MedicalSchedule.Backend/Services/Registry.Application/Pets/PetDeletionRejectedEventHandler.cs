using BuildingBlocks.Persistence.Abstractions;
using Microsoft.Extensions.Logging;
using Registry.Application.Events;
using Registry.Features.Abstractions;
using SharedKernel.Abstractions;

namespace Registry.Features.Pets;

public sealed class PetDeletionRejectedEventHandler(
    IPetRepository petRepository,
    IUnitOfWork unitOfWork,
    ILogger<PetDeletionRejectedEventHandler> logger) : IDomainEventHandler<PetDeletionRejectedEvent>
{
    public async Task HandleAsync(PetDeletionRejectedEvent @event, CancellationToken cancellationToken = default)
    {
        var pet = await petRepository.GetByIdAsync(@event.PetId, cancellationToken);
        if (pet is null)
        {
            logger.LogWarning("PetDeletionRejectedEvent ignored: pet {PetId} not found.", @event.PetId);
            return;
        }

        pet.RejectDeletion(@event.Reason);

        await petRepository.UpdateAsync(pet, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Pet {PetId} deletion rejected. Reason: {Reason}", @event.PetId, @event.Reason);
    }
}

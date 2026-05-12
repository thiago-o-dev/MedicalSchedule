using MedicalSchedule.Application.Abstractions;
using MedicalSchedule.Application.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace MedicalSchedule.Application.Features.Registration.Pets;

public class DeactivatePetCommandHandler(
    IUnitOfWork unitOfWork,
    IDomainEventDispatcher dispatcher)
{
    public async Task HandleAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var pet = await unitOfWork.Pets.FirstOrDefaultAsync(p => p.Id == id, cancellationToken)
            ?? throw new NotFoundException($"Pet '{id}' not found.");

        // Raises PetDeactivationRequestedEvent — the Consultations BC handler
        // will reject this if there are future scheduled consultations.
        pet.RequestDeactivation();
        await dispatcher.DispatchAsync(pet.DomainEvents, cancellationToken);
        pet.ClearDomainEvents();

        pet.Deactivate();
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

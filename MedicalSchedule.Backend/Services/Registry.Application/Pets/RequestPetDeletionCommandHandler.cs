using BuildingBlocks.Persistence.Abstractions;
using Registry.Features.Abstractions;
using SharedKernel.Abstractions;
using SharedKernel.Exceptions;

namespace Registry.Features.Pets;

public sealed class RequestPetDeletionCommandHandler(
    IPetRepository petRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<RequestPetDeletionCommand>
{
    public async Task HandleAsync(RequestPetDeletionCommand command, CancellationToken cancellationToken = default)
    {
        var pet = await petRepository.GetByIdAsync(command.PetId, cancellationToken)
            ?? throw new NotFoundException($"Pet '{command.PetId}' not found.");

        pet.RequestDeletion();

        await petRepository.UpdateAsync(pet, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

using BuildingBlocks.Persistence.Abstractions;
using Registry.Features.Shared;
using SharedKernel.Abstractions;
using SharedKernel.Exceptions;

namespace Registry.Features.Pets;

public sealed class RemovePetOwnerCommandHandler(
    IPetRepository petRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<RemovePetOwnerCommand>
{
    public async Task HandleAsync(RemovePetOwnerCommand command, CancellationToken cancellationToken = default)
    {
        var pet = await petRepository.GetByIdAsync(command.PetId, cancellationToken)
            ?? throw new NotFoundException($"Pet '{command.PetId}' not found.");

        pet.RemoveOwner(command.OwnerId);

        await petRepository.UpdateAsync(pet, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

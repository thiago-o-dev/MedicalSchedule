using BuildingBlocks.Persistence.Abstractions;
using Registry.Features.Shared;
using SharedKernel.Abstractions;
using SharedKernel.Exceptions;

namespace Registry.Features.Pets;

public sealed class AddPetOwnerCommandHandler(
    IPetRepository petRepository,
    IOwnerRepository ownerRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<AddPetOwnerCommand>
{
    public async Task HandleAsync(AddPetOwnerCommand command, CancellationToken cancellationToken = default)
    {
        var pet = await petRepository.GetByIdAsync(command.PetId, cancellationToken)
            ?? throw new NotFoundException($"Pet '{command.PetId}' not found.");

        var ownerExists = await ownerRepository.GetByIdAsync(command.OwnerId, cancellationToken);
        if (ownerExists is null)
            throw new NotFoundException($"Owner '{command.OwnerId}' not found.");

        pet.AddOwner(command.OwnerId, command.IsPrimaryOwner);

        await petRepository.UpdateAsync(pet, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

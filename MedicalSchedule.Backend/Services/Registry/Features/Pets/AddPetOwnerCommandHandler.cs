using BuildingBlocks.Persistence.Abstractions;
using Registry.Features.Shared;
using SharedKernel.Abstractions;
using SharedKernel.Exceptions;

namespace Registry.Features.Pets;

public sealed class AddPetOwnerCommandHandler(
    IPetRepository petRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<AddPetOwnerCommand>
{
    public async Task HandleAsync(AddPetOwnerCommand command, CancellationToken cancellationToken = default)
    {
        var pet = await petRepository.GetByIdAsync(command.PetId, cancellationToken)
            ?? throw new NotFoundException($"Pet '{command.PetId}' not found.");

        pet.AddOwner(command.OwnerId, command.IsPrimaryOwner);

        await petRepository.UpdateAsync(pet, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

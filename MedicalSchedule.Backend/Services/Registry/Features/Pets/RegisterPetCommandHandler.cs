using BuildingBlocks.Persistence.Abstractions;
using Registry.Domain.Entities;
using Registry.Features.Shared;
using SharedKernel.Abstractions;

namespace Registry.Features.Pets;

public sealed class RegisterPetCommandHandler(
    IPetRepository petRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<RegisterPetCommand, Guid>
{
    public async Task<Guid> HandleAsync(RegisterPetCommand command, CancellationToken cancellationToken = default)
    {
        var pet = Pet.Create(
            command.Name,
            command.Species,
            command.Breed,
            command.BirthDate,
            command.PrimaryOwnerId);

        await petRepository.AddAsync(pet, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return pet.Id;
    }
}

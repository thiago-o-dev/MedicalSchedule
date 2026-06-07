using BuildingBlocks.Persistence.Abstractions;
using Registry.Domain.Entities;
using Registry.Features.Abstractions;
using SharedKernel.Abstractions;

namespace Registry.Features.Vets;

public sealed class CreateVetCommandHandler(
    IVetRepository vetRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<CreateVetCommand, Guid>
{
    public async Task<Guid> HandleAsync(CreateVetCommand command, CancellationToken cancellationToken = default)
    {
        var vet = Vet.Create(command.Name, command.Crm, command.Specialty, command.Email);

        await vetRepository.AddAsync(vet, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return vet.Id;
    }
}

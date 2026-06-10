using BuildingBlocks.Persistence.Abstractions;
using Registry.Domain.Entities;
using Registry.Domain.Exceptions;
using Registry.Features.Abstractions;
using SharedKernel.Abstractions;

namespace Registry.Features.Vets;

public sealed class CreateVetCommandHandler(
    IVetRepository vetRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<CreateVetCommand, Guid>
{
    public async Task<Guid> HandleAsync(CreateVetCommand command, CancellationToken cancellationToken = default)
    {
        if (await vetRepository.GetByCrmAsync(command.Crm, cancellationToken) is not null)
            throw new DuplicateVetException($"A vet with CRM '{command.Crm}' already exists.");

        if (await vetRepository.GetByEmailAsync(command.Email, cancellationToken) is not null)
            throw new DuplicateVetException($"A vet with email '{command.Email}' already exists.");

        var vet = Vet.Create(command.Name, command.Crm, command.Specialty, command.Email);

        await vetRepository.AddAsync(vet, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return vet.Id;
    }
}

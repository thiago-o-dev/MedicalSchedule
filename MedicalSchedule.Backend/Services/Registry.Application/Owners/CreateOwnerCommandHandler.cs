using BuildingBlocks.Persistence.Abstractions;
using Registry.Domain.Entities;
using Registry.Domain.Exceptions;
using Registry.Features.Abstractions;
using SharedKernel.Abstractions;
using SharedKernel.Exceptions;

namespace Registry.Features.Owners;

public sealed class CreateOwnerCommandHandler(
    IOwnerRepository ownerRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<CreateOwnerCommand, Guid>
{
    public async Task<Guid> HandleAsync(CreateOwnerCommand command, CancellationToken cancellationToken = default)
    {
        if (await ownerRepository.GetByEmailAsync(command.Email, cancellationToken) is not null)
            throw new DuplicateOwnerException($"An owner with email '{command.Email}' already exists.");

        if (await ownerRepository.GetByCpfAsync(command.Cpf, cancellationToken) is not null)
            throw new DuplicateOwnerException($"An owner with CPF '{command.Cpf}' already exists.");

        var owner = Owner.Create(command.Name, command.Cpf, command.Phone, command.Email);

        await ownerRepository.AddAsync(owner, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return owner.Id;
    }
}

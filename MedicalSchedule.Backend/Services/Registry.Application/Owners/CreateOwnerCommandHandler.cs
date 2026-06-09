using BuildingBlocks.Persistence.Abstractions;
using Registry.Domain.Entities;
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
        if (await ownerRepository.FindDuplicateAsync(command.Cpf, command.Email) != null)
        {
            throw new BusinessLogicException("There can't be more than one owner with a cpf or email");
        }

        var owner = Owner.Create(command.Name, command.Cpf, command.Phone, command.Email);

        await ownerRepository.AddAsync(owner, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return owner.Id;
    }
}

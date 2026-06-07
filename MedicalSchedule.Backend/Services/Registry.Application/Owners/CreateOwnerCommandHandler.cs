using BuildingBlocks.Persistence.Abstractions;
using Registry.Domain.Entities;
using Registry.Features.Abstractions;
using SharedKernel.Abstractions;

namespace Registry.Features.Owners;

public sealed class CreateOwnerCommandHandler(
    IOwnerRepository ownerRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<CreateOwnerCommand, Guid>
{
    public async Task<Guid> HandleAsync(CreateOwnerCommand command, CancellationToken cancellationToken = default)
    {
        var owner = Owner.Create(command.Name, command.Cpf, command.Phone, command.Email);

        await ownerRepository.AddAsync(owner, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return owner.Id;
    }
}

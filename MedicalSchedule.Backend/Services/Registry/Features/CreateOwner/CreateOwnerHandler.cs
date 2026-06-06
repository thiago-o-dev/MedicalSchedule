using MediatR;
using Registry.Domain.Entities;
using Registry.Features.Shared;

namespace Registry.Features.CreateOwner;

public sealed class CreateOwnerHandler : IRequestHandler<CreateOwnerCommand, Guid>
{
    private readonly IOwnerRepository _repository;

    public CreateOwnerHandler(IOwnerRepository repository)
    {
        _repository = repository;
    }

    public async Task<Guid> Handle(
        CreateOwnerCommand request,
        CancellationToken cancellationToken)
    {
        var owner = Owner.Create(
            request.Name,
            request.Cpf,
            request.Phone,
            request.Email);

        await _repository.AddAsync(owner);

        return owner.Id;
    }
}
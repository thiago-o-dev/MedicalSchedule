using MediatR;
using Registry.Features.Shared;

namespace Registry.Features.AddPetOwner;

public sealed class AddPetOwnerHandler
    : IRequestHandler<AddPetOwnerCommand>
{
    private readonly IPetRepository _repository;

    public AddPetOwnerHandler(IPetRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(
        AddPetOwnerCommand request,
        CancellationToken cancellationToken)
    {
        var pet = await _repository.GetByIdAsync(request.PetId);

        pet.AddOwner(request.OwnerId, request.IsPrimaryOwner);

        await _repository.UpdateAsync(pet);
    }
}
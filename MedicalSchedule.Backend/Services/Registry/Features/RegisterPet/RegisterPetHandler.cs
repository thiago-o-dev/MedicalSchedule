using MediatR;
using Registry.Domain.Entities;
using Registry.Features.Shared;

namespace Registry.Features.RegisterPet;

public sealed class RegisterPetHandler
    : IRequestHandler<RegisterPetCommand, Guid>
{
    private readonly IPetRepository _repository;

    public RegisterPetHandler(IPetRepository repository)
    {
        _repository = repository;
    }

    public async Task<Guid> Handle(
        RegisterPetCommand request,
        CancellationToken cancellationToken)
    {
        var pet = Pet.Create(
            request.Name,
            request.Species,
            request.Breed,
            request.BirthDate,
            request.PrimaryOwnerId);

        await _repository.AddAsync(pet);

        return pet.Id;
    }
}
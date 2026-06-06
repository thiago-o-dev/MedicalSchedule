using MediatR;
using Registry.Domain.Enums;

namespace Registry.Features.RegisterPet;

public sealed record RegisterPetCommand(
    string Name,
    PetSpecies Species,
    string Breed,
    DateOnly BirthDate,
    Guid PrimaryOwnerId) : IRequest<Guid>;
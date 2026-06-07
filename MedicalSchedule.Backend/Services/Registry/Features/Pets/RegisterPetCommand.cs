using Registry.Domain.Enums;

namespace Registry.Features.Pets;

public sealed record RegisterPetCommand(
    string Name,
    PetSpecies Species,
    string Breed,
    DateOnly BirthDate,
    Guid PrimaryOwnerId);

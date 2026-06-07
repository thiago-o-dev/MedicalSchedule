using Registry.Domain.Enums;

namespace Registry.Features.Pets;

public sealed record GetPetByIdQuery(Guid PetId);

public sealed record PetResponse(
    Guid Id,
    string Name,
    PetSpecies Species,
    string Breed,
    DateOnly BirthDate,
    bool IsActive);

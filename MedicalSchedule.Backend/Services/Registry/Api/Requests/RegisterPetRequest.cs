using Registry.Domain.Enums;

namespace Registry.Api.Requests;

public sealed record RegisterPetRequest(
    string Name,
    PetSpecies Species,
    string Breed,
    DateOnly BirthDate,
    Guid PrimaryOwnerId);

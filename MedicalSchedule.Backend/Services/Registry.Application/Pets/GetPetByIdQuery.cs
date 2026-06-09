using Registry.Domain.Enums;

namespace Registry.Features.Pets;

public sealed record GetPetByIdQuery(Guid PetId);

public sealed record PetResponse(
    Guid Id,
    string Name,
    PetSpecies Species,
    string Breed,
    DateOnly BirthDate,
    bool IsActive,
    PetDeletionStatus DeletionStatus,
    string? DeletionRejectionReason,
    IReadOnlyList<PetOwnershipResponse> Ownerships);

public sealed record PetOwnershipResponse(Guid OwnerId, bool IsPrimaryOwner);

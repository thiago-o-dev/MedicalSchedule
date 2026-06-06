namespace Registry.Features.Pets;

public sealed record AddPetOwnerCommand(Guid PetId, Guid OwnerId, bool IsPrimaryOwner);

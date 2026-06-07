namespace Registry.Features.Pets;

public sealed record RemovePetOwnerCommand(Guid PetId, Guid OwnerId);

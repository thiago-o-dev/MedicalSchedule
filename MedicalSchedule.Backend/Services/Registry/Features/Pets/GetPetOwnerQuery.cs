namespace Registry.Features.Pets;

public sealed record GetPetOwnerQuery(Guid PetId);

public sealed record OwnerContactResponse(string Name, string Email);

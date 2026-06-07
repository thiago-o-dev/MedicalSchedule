namespace Registry.Features.Pets;

public sealed record GetAllPetsQuery(Guid? OwnerId = null);

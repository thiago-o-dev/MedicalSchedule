namespace Registry.Api.Requests;

public sealed record AddPetOwnerRequest(Guid OwnerId, bool IsPrimaryOwner);

using MediatR;

namespace Registry.Features.AddPetOwner;

public sealed record AddPetOwnerCommand(
    Guid PetId,
    Guid OwnerId,
    bool IsPrimaryOwner) : IRequest;
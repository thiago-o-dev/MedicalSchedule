using SharedKernel.Abstractions;
using SharedKernel.Exceptions;

namespace Registry.Domain.Entities;

public sealed class PetOwnership
{
    // vai ser deletavel por isso n usamos o lifecycle
    public Guid OwnerId { get; private set; }
    public Guid PetId { get; private set; }
    public bool IsPrimaryOwner { get; private set; }

    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    private PetOwnership() { }

    public static PetOwnership Create(
        Guid petId,
        Guid ownerId,
        bool isPrimaryOwner)
    {
        return new PetOwnership
        {
            PetId = petId,
            OwnerId = ownerId,
            IsPrimaryOwner = isPrimaryOwner
        };
    }
}

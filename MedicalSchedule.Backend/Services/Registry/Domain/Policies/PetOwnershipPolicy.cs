using Registry.Domain.Entities;
using SharedKernel.Exceptions;
using Registry.Domain.Exceptions;

namespace Registry.Domain.Policies;

public static class PetOwnershipPolicy
{
    public static void EnsureCanAddOwner(
        IEnumerable<PetOwnership> ownerships,
        Guid ownerId,
        bool isPrimary)
    {
        if (ownerships.Any(x => x.OwnerId == ownerId))
        {
            throw new DuplicateOwnerException("Owner already assigned.");
        }

        if (isPrimary &&
            ownerships.Any(x => x.IsPrimaryOwner))
        {
            throw new DomainValidationException("Primary owner already exists.");
        }
    }

    public static void EnsureCanRemoveOwner(
        IEnumerable<PetOwnership> ownerships,
        PetOwnership ownership)
    {
        if (ownership.IsPrimaryOwner &&
            ownerships.Count() == 1)
        {
            throw new DomainValidationException("Cannot remove last primary owner.");
        }
    }
}
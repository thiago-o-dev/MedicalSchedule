using Registry.Domain.Enums;
using Registry.Domain.Events;
using Registry.Domain.Policies;
using SharedKernel.Abstractions;
using SharedKernel.Exceptions;

namespace Registry.Domain.Entities;

public class Pet : LifeCycleEntity
{
    private readonly List<PetOwnership> _ownerships = [];

    public string Name { get; private set; } = string.Empty;
    public PetSpecies Species { get; private set; }
    public string Breed { get; private set; } = string.Empty;
    public DateOnly BirthDate { get; private set; }
    public PetDeletionStatus DeletionStatus { get; private set; } = PetDeletionStatus.None;
    public string? DeletionRejectionReason { get; private set; }

    public IReadOnlyCollection<PetOwnership> Ownerships => _ownerships;

    private Pet() { }

    public static Pet Create(string name, PetSpecies species, string breed, DateOnly birthDate, Guid primaryOwnerId)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainValidationException("Pet name is required.");
        if (primaryOwnerId == Guid.Empty)
            throw new DomainValidationException("Owner is required.");

        var pet = new Pet
        {
            Id = Guid.NewGuid(),
            Name = name.Trim(),
            Species = species,
            Breed = breed.Trim(),
            BirthDate = birthDate,
        };

        pet.AddOwner(primaryOwnerId, true);

        return pet;
    }

    public void Update(string name, PetSpecies species, string breed, DateOnly birthDate)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainValidationException("Pet name is required.");

        Name = name.Trim();
        Species = species;
        Breed = breed.Trim();
        BirthDate = birthDate;
        Touch();
    }

    public void AddOwner( Guid ownerId, bool isPrimaryOwner = false)
    {
        PetOwnershipPolicy.EnsureCanAddOwner(
            _ownerships,
            ownerId,
            isPrimaryOwner);

        _ownerships.Add(
            PetOwnership.Create(
                Id,
                ownerId,
                isPrimaryOwner));
    }

    public void RemoveOwner(Guid ownerId)
    {
        var ownership = _ownerships.FirstOrDefault(x => x.OwnerId == ownerId);

        if (ownership is null)
            throw new NotFoundException($"Ownership for owner '{ownerId}' not found.");

        PetOwnershipPolicy.EnsureCanRemoveOwner(
            _ownerships,
            ownership);

        _ownerships.Remove(ownership);
    }

    public void RequestDeletion()
    {
        if (DeletionStatus == PetDeletionStatus.Deleted)
            throw new DomainValidationException("Pet is already deleted.");
        if (DeletionStatus == PetDeletionStatus.PendingDeletion)
            throw new DomainValidationException("Pet deletion is already pending review.");

        DeletionStatus = PetDeletionStatus.PendingDeletion;
        DeletionRejectionReason = null;
        Touch();

        RaiseDomainEvent(new PetDeletionRequestedEvent(Id));
    }

    public void ConfirmDeletion()
    {
        if (DeletionStatus != PetDeletionStatus.PendingDeletion)
            return;

        DeletionStatus = PetDeletionStatus.Deleted;
        DeletionRejectionReason = null;
        Deactivate();
    }

    public void RejectDeletion(string reason)
    {
        if (DeletionStatus != PetDeletionStatus.PendingDeletion)
            return;

        DeletionStatus = PetDeletionStatus.None;
        DeletionRejectionReason = reason;
        Touch();
    }
}

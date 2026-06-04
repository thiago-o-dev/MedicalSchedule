using MedicalSchedule.Domain.Abstractions;
using MedicalSchedule.Domain.Enums;
using MedicalSchedule.Domain.Events.Registration;
using MedicalSchedule.Domain.Exceptions;

namespace MedicalSchedule.Domain.Entities.Registration;

public class Pet : LifeCycleEntity
{
    public string Name { get; private set; } = string.Empty;
    public PetSpecies Species { get; private set; }
    public string Breed { get; private set; } = string.Empty;
    public DateOnly BirthDate { get; private set; }
    public Guid OwnerId { get; private set; }
    public Owner Owner { get; private set; } = null!;

    private Pet() { }

    public static Pet Create(string name, PetSpecies species, string breed, DateOnly birthDate, Guid ownerId)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainValidationException("Pet name is required.");
        if (ownerId == Guid.Empty)
            throw new DomainValidationException("Owner is required.");

        return new Pet
        {
            Id = Guid.NewGuid(),
            Name = name.Trim(),
            Species = species,
            Breed = breed.Trim(),
            BirthDate = birthDate,
            OwnerId = ownerId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
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

    // Raises a cross-BC validation event. The handler in the Consultations BC
    // will reject this if the pet has future scheduled consultations.
    public void RequestDeactivation()
        => RaiseDomainEvent(new PetDeactivationRequestedEvent(Id));

    public new void Deactivate()
        => base.Deactivate();
}

using Registry.Domain.Entities;
using Registry.Domain.Enums;
using Registry.Domain.Events;
using SharedKernel.Exceptions;

namespace MedicalSchedule.Backend.UnitTests.Registry;

public class PetDeletionDomainTests
{
    private static Pet CreatePet() =>
        Pet.Create("Rex", PetSpecies.Dog, "Labrador", new DateOnly(2020, 1, 1), Guid.NewGuid());

    [Fact]
    public void RequestDeletion_FromNone_ShouldRaiseEventAndSetPending()
    {
        var pet = CreatePet();

        pet.RequestDeletion();

        pet.DeletionStatus.Should().Be(PetDeletionStatus.PendingDeletion);
        pet.DomainEvents.Should().ContainSingle(e => e is PetDeletionRequestedEvent);
    }

    [Fact]
    public void RequestDeletion_WhenAlreadyPending_ShouldThrow()
    {
        var pet = CreatePet();
        pet.RequestDeletion();

        var act = () => pet.RequestDeletion();

        act.Should().Throw<DomainValidationException>().WithMessage("*pending*");
    }

    [Fact]
    public void ConfirmDeletion_FromPending_ShouldSoftDelete()
    {
        var pet = CreatePet();
        pet.RequestDeletion();

        pet.ConfirmDeletion();

        pet.DeletionStatus.Should().Be(PetDeletionStatus.Deleted);
        pet.IsActive.Should().BeFalse();
    }

    [Fact]
    public void ConfirmDeletion_FromNone_ShouldBeIdempotent()
    {
        var pet = CreatePet();

        pet.ConfirmDeletion();

        pet.DeletionStatus.Should().Be(PetDeletionStatus.None);
        pet.IsActive.Should().BeTrue();
    }

    [Fact]
    public void RejectDeletion_FromPending_ShouldRestoreAndRecordReason()
    {
        var pet = CreatePet();
        pet.RequestDeletion();

        pet.RejectDeletion("has future consultation");

        pet.DeletionStatus.Should().Be(PetDeletionStatus.None);
        pet.DeletionRejectionReason.Should().Be("has future consultation");
        pet.IsActive.Should().BeTrue();
    }
}

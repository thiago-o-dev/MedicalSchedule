using BuildingBlocks.Persistence.Abstractions;
using Registry.Domain.Entities;
using Registry.Domain.Enums;
using Registry.Features.Abstractions;
using Registry.Features.Pets;
using SharedKernel.Exceptions;

namespace MedicalSchedule.Backend.UnitTests.Registry;

public class RequestPetDeletionCommandHandlerTests
{
    [Fact]
    public async Task Handle_WhenPetNotFound_ShouldThrowNotFound()
    {
        var petRepo = Substitute.For<IPetRepository>();
        var uow = Substitute.For<IUnitOfWork>();
        petRepo.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((Pet?)null);

        var handler = new RequestPetDeletionCommandHandler(petRepo, uow);

        var act = async () => await handler.HandleAsync(new RequestPetDeletionCommand(Guid.NewGuid()));

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_WhenPetExists_ShouldTransitionToPendingAndSave()
    {
        var pet = Pet.Create("Rex", PetSpecies.Dog, "Labrador", new DateOnly(2020, 1, 1), Guid.NewGuid());

        var petRepo = Substitute.For<IPetRepository>();
        var uow = Substitute.For<IUnitOfWork>();
        petRepo.GetByIdAsync(pet.Id, Arg.Any<CancellationToken>()).Returns(pet);

        var handler = new RequestPetDeletionCommandHandler(petRepo, uow);

        await handler.HandleAsync(new RequestPetDeletionCommand(pet.Id));

        pet.DeletionStatus.Should().Be(PetDeletionStatus.PendingDeletion);
        await petRepo.Received(1).UpdateAsync(pet, Arg.Any<CancellationToken>());
        await uow.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}

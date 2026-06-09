using BuildingBlocks.Messaging.Abstractions;
using Microsoft.Extensions.Logging.Abstractions;
using Scheduling.Contracts.Events;
using Scheduling.Domain.Entities;
using Scheduling.Features.Sagas;

namespace MedicalSchedule.Backend.UnitTests.Scheduling;

public class PetDeletionRequestedSagaHandlerTests
{
    [Fact]
    public async Task Handle_WhenNoFutureConsultations_ShouldPublishApproved()
    {
        await using var uow = InMemorySchedulingUnitOfWork.Create();
        var publisher = Substitute.For<IMessagePublisher>();
        var handler = new PetDeletionRequestedSagaHandler(uow, publisher, NullLogger<PetDeletionRequestedSagaHandler>.Instance);

        var petId = Guid.NewGuid();

        await handler.HandleAsync(new PetDeletionRequestedEvent(petId));

        await publisher.Received(1).PublishAsync(
            Arg.Is<PetDeletionApprovedEvent>(e => e.PetId == petId),
            Arg.Any<CancellationToken>());
        await publisher.DidNotReceive().PublishAsync(
            Arg.Any<PetDeletionRejectedEvent>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenFutureScheduledConsultationExists_ShouldPublishRejected()
    {
        await using var uow = InMemorySchedulingUnitOfWork.Create();

        var petId = Guid.NewGuid();
        var future = Consultation.Schedule(petId, Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow.AddDays(5), null);
        uow.Consultations.Add(future);
        await uow.SaveChangesAsync();

        var publisher = Substitute.For<IMessagePublisher>();
        var handler = new PetDeletionRequestedSagaHandler(uow, publisher, NullLogger<PetDeletionRequestedSagaHandler>.Instance);

        await handler.HandleAsync(new PetDeletionRequestedEvent(petId));

        await publisher.Received(1).PublishAsync(
            Arg.Is<PetDeletionRejectedEvent>(e => e.PetId == petId && e.Reason.Contains("future")),
            Arg.Any<CancellationToken>());
        await publisher.DidNotReceive().PublishAsync(
            Arg.Any<PetDeletionApprovedEvent>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenOnlyCancelledConsultations_ShouldPublishApproved()
    {
        await using var uow = InMemorySchedulingUnitOfWork.Create();

        var petId = Guid.NewGuid();
        var cancelled = Consultation.Schedule(petId, Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow.AddDays(5), null);
        cancelled.Cancel();
        uow.Consultations.Add(cancelled);
        await uow.SaveChangesAsync();

        var publisher = Substitute.For<IMessagePublisher>();
        var handler = new PetDeletionRequestedSagaHandler(uow, publisher, NullLogger<PetDeletionRequestedSagaHandler>.Instance);

        await handler.HandleAsync(new PetDeletionRequestedEvent(petId));

        await publisher.Received(1).PublishAsync(
            Arg.Any<PetDeletionApprovedEvent>(),
            Arg.Any<CancellationToken>());
    }
}

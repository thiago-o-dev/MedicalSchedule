using Caching.Redis;
using Scheduling.Domain.Entities;
using Scheduling.Features.Consultations;
using SharedKernel.Exceptions;

namespace MedicalSchedule.Backend.UnitTests.Scheduling;

public class ScheduleConsultationCommandHandlerTests
{
    private static ISlotLockService LockAlwaysGranted()
    {
        var mock = Substitute.For<ISlotLockService>();
        mock.TryAcquireSlotLockAsync(Arg.Any<Guid>(), Arg.Any<DateTime>(), Arg.Any<CancellationToken>())
            .Returns(true);
        return mock;
    }

    [Fact]
    public async Task Schedule_WhenNoConflict_ShouldPersistConsultation()
    {
        await using var uow = InMemorySchedulingUnitOfWork.Create();
        var handler = new ScheduleConsultationCommandHandler(uow, LockAlwaysGranted());

        var command = new ScheduleConsultationCommand(
            PetId: Guid.NewGuid(),
            VetId: Guid.NewGuid(),
            OwnerId: Guid.NewGuid(),
            ScheduledAt: DateTime.UtcNow.AddDays(3),
            Notes: "Routine checkup");

        var id = await handler.HandleAsync(command);

        id.Should().NotBe(Guid.Empty);
        uow.Consultations.Should().ContainSingle(c => c.Id == id);
    }

    [Fact]
    public async Task Schedule_WhenVetAlreadyBookedAtSameTime_ShouldThrowConflict()
    {
        await using var uow = InMemorySchedulingUnitOfWork.Create();

        var vetId = Guid.NewGuid();
        var slot = DateTime.SpecifyKind(DateTime.UtcNow.AddDays(2), DateTimeKind.Utc);

        var existing = Consultation.Schedule(Guid.NewGuid(), vetId, Guid.NewGuid(), slot, null);
        uow.Consultations.Add(existing);
        await uow.SaveChangesAsync();

        var handler = new ScheduleConsultationCommandHandler(uow, LockAlwaysGranted());
        var command = new ScheduleConsultationCommand(Guid.NewGuid(), vetId, Guid.NewGuid(), slot, null);

        var act = async () => await handler.HandleAsync(command);

        await act.Should().ThrowAsync<ConflictException>()
            .WithMessage("*already has a consultation*");
    }

    [Fact]
    public async Task Schedule_WhenSlotIsLocked_ShouldThrowConflict()
    {
        await using var uow = InMemorySchedulingUnitOfWork.Create();

        var slotLock = Substitute.For<ISlotLockService>();
        slotLock.TryAcquireSlotLockAsync(Arg.Any<Guid>(), Arg.Any<DateTime>(), Arg.Any<CancellationToken>())
            .Returns(false);

        var handler = new ScheduleConsultationCommandHandler(uow, slotLock);
        var command = new ScheduleConsultationCommand(
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow.AddDays(1), null);

        var act = async () => await handler.HandleAsync(command);

        await act.Should().ThrowAsync<ConflictException>()
            .WithMessage("*temporarily reserved*");
    }

    [Fact]
    public async Task Schedule_WhenVetHasCancelledConsultationAtSameTime_ShouldSucceed()
    {
        await using var uow = InMemorySchedulingUnitOfWork.Create();

        var vetId = Guid.NewGuid();
        var slot = DateTime.SpecifyKind(DateTime.UtcNow.AddDays(2), DateTimeKind.Utc);

        var cancelled = Consultation.Schedule(Guid.NewGuid(), vetId, Guid.NewGuid(), slot, null);
        cancelled.Cancel();
        uow.Consultations.Add(cancelled);
        await uow.SaveChangesAsync();

        var handler = new ScheduleConsultationCommandHandler(uow, LockAlwaysGranted());
        var id = await handler.HandleAsync(
            new ScheduleConsultationCommand(Guid.NewGuid(), vetId, Guid.NewGuid(), slot, null));

        id.Should().NotBe(Guid.Empty);
    }
}

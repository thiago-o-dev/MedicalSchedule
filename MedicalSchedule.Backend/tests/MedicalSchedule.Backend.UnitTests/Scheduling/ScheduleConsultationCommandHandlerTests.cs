using Scheduling.Domain.Entities;
using Scheduling.Features.Consultations;
using SharedKernel.Exceptions;

namespace MedicalSchedule.Backend.UnitTests.Scheduling;

public class ScheduleConsultationCommandHandlerTests
{
    [Fact]
    public async Task Schedule_WhenNoConflict_ShouldPersistConsultation()
    {
        await using var uow = InMemorySchedulingUnitOfWork.Create();
        var handler = new ScheduleConsultationCommandHandler(uow);

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

        var handler = new ScheduleConsultationCommandHandler(uow);
        var command = new ScheduleConsultationCommand(Guid.NewGuid(), vetId, Guid.NewGuid(), slot, null);

        var act = async () => await handler.HandleAsync(command);

        await act.Should().ThrowAsync<ConflictException>()
            .WithMessage("*already has a consultation*");
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

        var handler = new ScheduleConsultationCommandHandler(uow);
        var id = await handler.HandleAsync(
            new ScheduleConsultationCommand(Guid.NewGuid(), vetId, Guid.NewGuid(), slot, null));

        id.Should().NotBe(Guid.Empty);
    }
}

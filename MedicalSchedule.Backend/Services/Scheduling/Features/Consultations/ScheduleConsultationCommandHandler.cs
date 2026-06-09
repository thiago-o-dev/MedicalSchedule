using Caching.Redis;
using Microsoft.EntityFrameworkCore;
using Scheduling.Domain.Entities;
using Scheduling.Domain.Enums;
using Scheduling.Infrastructure;
using SharedKernel.Abstractions;
using SharedKernel.Exceptions;

namespace Scheduling.Features.Consultations;

public sealed class ScheduleConsultationCommandHandler(
    ISchedulingUnitOfWork unitOfWork,
    ISlotLockService slotLockService)
    : ICommandHandler<ScheduleConsultationCommand, Guid>
{
    public async Task<Guid> HandleAsync(ScheduleConsultationCommand command, CancellationToken cancellationToken = default)
    {
        var scheduledAt = DateTime.SpecifyKind(command.ScheduledAt, DateTimeKind.Utc);

        var locked = await slotLockService.TryAcquireSlotLockAsync(command.VetId, scheduledAt, cancellationToken);
        if (!locked)
            throw new ConflictException("This slot is temporarily reserved. Please try again in a moment.");

        var conflictExists = await unitOfWork.Consultations
            .AnyAsync(c =>
                c.VetId == command.VetId &&
                c.Status == ConsultationStatus.Scheduled &&
                c.ScheduledAt == scheduledAt,
                cancellationToken);

        if (conflictExists)
            throw new ConflictException("This vet already has a consultation scheduled at the specified time.");

        var consultation = Consultation.Schedule(command.PetId, command.VetId, command.OwnerId, scheduledAt, command.Notes);

        unitOfWork.Consultations.Add(consultation);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return consultation.Id;
    }
}

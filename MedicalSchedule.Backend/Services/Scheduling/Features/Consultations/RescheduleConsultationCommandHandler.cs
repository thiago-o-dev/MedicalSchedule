using Caching.Redis;
using Microsoft.EntityFrameworkCore;
using Scheduling.Infrastructure;
using SharedKernel.Abstractions;
using SharedKernel.Exceptions;

namespace Scheduling.Features.Consultations;

public sealed class RescheduleConsultationCommandHandler(
    ISchedulingUnitOfWork unitOfWork,
    ISlotLockService slotLockService)
    : ICommandHandler<RescheduleConsultationCommand>
{
    public async Task HandleAsync(RescheduleConsultationCommand command, CancellationToken cancellationToken = default)
    {
        var consultation = await unitOfWork.Consultations
            .FirstOrDefaultAsync(c => c.Id == command.ConsultationId, cancellationToken)
            ?? throw new NotFoundException($"Consultation '{command.ConsultationId}' not found.");

        var newScheduledAt = DateTime.SpecifyKind(command.NewScheduledAt, DateTimeKind.Utc);

        var locked = await slotLockService.TryAcquireSlotLockAsync(consultation.VetId, newScheduledAt, cancellationToken);
        if (!locked)
            throw new ConflictException("This slot is temporarily reserved. Please try again in a moment.");

        consultation.Reschedule(newScheduledAt);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

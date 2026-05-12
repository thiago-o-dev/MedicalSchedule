using MedicalSchedule.Application.Abstractions;
using MedicalSchedule.Application.Exceptions;
using MedicalSchedule.Domain.Abstractions;
using MedicalSchedule.Domain.Enums;
using MedicalSchedule.Domain.Events.Registration;
using Microsoft.EntityFrameworkCore;

namespace MedicalSchedule.Application.EventHandlers.Consultations;

// Consultations BC handles this Registration BC event to enforce the cross-context
// invariant: a pet with future consultations cannot be deactivated.
public class PetDeactivationRequestedEventHandler(IUnitOfWork unitOfWork)
    : IDomainEventHandler<PetDeactivationRequestedEvent>
{
    public async Task HandleAsync(
        PetDeactivationRequestedEvent @event,
        CancellationToken cancellationToken = default)
    {
        var hasFutureConsultations = await unitOfWork.Consultations
            .AnyAsync(c =>
                c.PetId == @event.PetId &&
                c.Status == ConsultationStatus.Scheduled &&
                c.ScheduledAt > DateTime.UtcNow,
                cancellationToken);

        if (hasFutureConsultations)
            throw new BusinessLogicException(
                "Cannot deactivate a pet that has future scheduled consultations.");
    }
}

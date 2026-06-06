using SharedKernel.Abstractions;

namespace Scheduling.Domain.Events;

public record ConsultationRescheduledEvent(
    Guid ConsultationId,
    Guid PetId,
    Guid VetId,
    DateTime PreviousScheduledAt,
    DateTime NewScheduledAt) : IDomainEvent;

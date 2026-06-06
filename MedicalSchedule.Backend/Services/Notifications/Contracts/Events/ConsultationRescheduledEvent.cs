using SharedKernel.Abstractions;

namespace Notifications.Contracts.Events;

public record ConsultationRescheduledEvent(
    Guid ConsultationId,
    Guid PetId,
    Guid VetId,
    DateTime PreviousScheduledAt,
    DateTime NewScheduledAt) : IDomainEvent;

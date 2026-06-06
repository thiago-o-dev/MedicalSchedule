using SharedKernel.Abstractions;

namespace Notifications.Contracts.Events;

public record ConsultationScheduledEvent(
    Guid ConsultationId,
    Guid PetId,
    Guid VetId,
    DateTime ScheduledAt) : IDomainEvent;

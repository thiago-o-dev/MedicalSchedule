using SharedKernel.Abstractions;

namespace Notifications.Contracts.Events;

public record ConsultationCancelledEvent(
    Guid ConsultationId,
    Guid PetId,
    Guid VetId,
    DateTime ScheduledAt) : IDomainEvent;

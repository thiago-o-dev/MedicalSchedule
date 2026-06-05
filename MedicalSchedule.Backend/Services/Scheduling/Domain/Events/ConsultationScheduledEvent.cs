using SharedKernel.Abstractions;

namespace Scheduling.Domain.Events;

public record ConsultationScheduledEvent(
    Guid ConsultationId,
    Guid PetId,
    Guid VetId,
    DateTime ScheduledAt) : IDomainEvent;

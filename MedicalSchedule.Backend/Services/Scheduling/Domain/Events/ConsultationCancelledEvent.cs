using SharedKernel.Abstractions;

namespace Scheduling.Domain.Events;

public record ConsultationCancelledEvent(
    Guid ConsultationId,
    Guid PetId,
    Guid VetId,
    DateTime ScheduledAt) : IDomainEvent;

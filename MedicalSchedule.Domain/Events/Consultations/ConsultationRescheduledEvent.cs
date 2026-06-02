using MedicalSchedule.Domain.Abstractions;

namespace MedicalSchedule.Domain.Events.Consultations;

public record ConsultationRescheduledEvent(
    Guid ConsultationId,
    Guid PetId,
    Guid VetId,
    DateTime PreviousScheduledAt,
    DateTime NewScheduledAt) : IDomainEvent;

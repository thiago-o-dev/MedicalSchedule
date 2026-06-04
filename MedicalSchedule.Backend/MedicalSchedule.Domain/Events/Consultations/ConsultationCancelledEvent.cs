using MedicalSchedule.Domain.Abstractions;

namespace MedicalSchedule.Domain.Events.Consultations;

public record ConsultationCancelledEvent(
    Guid ConsultationId,
    Guid PetId,
    Guid VetId,
    DateTime ScheduledAt) : IDomainEvent;

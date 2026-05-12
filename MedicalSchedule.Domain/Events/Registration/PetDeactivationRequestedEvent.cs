using MedicalSchedule.Domain.Abstractions;

namespace MedicalSchedule.Domain.Events.Registration;

public record PetDeactivationRequestedEvent(Guid PetId) : IDomainEvent;

using SharedKernel.Abstractions;

namespace Registry.Domain.Events;

public record PetDeactivationRequestedEvent(Guid PetId) : IDomainEvent;

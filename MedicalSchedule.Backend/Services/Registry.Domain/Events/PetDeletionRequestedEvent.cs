using SharedKernel.Abstractions;

namespace Registry.Domain.Events;

public record PetDeletionRequestedEvent(Guid PetId) : IDomainEvent;

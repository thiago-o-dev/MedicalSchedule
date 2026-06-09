using SharedKernel.Abstractions;

namespace Scheduling.Contracts.Events;

public record PetDeletionRequestedEvent(Guid PetId) : IDomainEvent;

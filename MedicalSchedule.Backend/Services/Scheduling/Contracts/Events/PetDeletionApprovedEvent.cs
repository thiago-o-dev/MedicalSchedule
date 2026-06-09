using SharedKernel.Abstractions;

namespace Scheduling.Contracts.Events;

public record PetDeletionApprovedEvent(Guid PetId) : IDomainEvent;

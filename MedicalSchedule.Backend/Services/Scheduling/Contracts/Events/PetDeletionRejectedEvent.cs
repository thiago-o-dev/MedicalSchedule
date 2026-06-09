using SharedKernel.Abstractions;

namespace Scheduling.Contracts.Events;

public record PetDeletionRejectedEvent(Guid PetId, string Reason) : IDomainEvent;

using SharedKernel.Abstractions;

namespace Registry.Application.Events;

public record PetDeletionRejectedEvent(Guid PetId, string Reason) : IDomainEvent;

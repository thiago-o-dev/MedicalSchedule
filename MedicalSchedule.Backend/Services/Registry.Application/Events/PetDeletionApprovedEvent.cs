using SharedKernel.Abstractions;

namespace Registry.Application.Events;

public record PetDeletionApprovedEvent(Guid PetId) : IDomainEvent;

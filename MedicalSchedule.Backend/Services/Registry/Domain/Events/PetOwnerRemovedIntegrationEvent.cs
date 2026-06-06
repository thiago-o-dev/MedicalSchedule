using SharedKernel.Abstractions;

namespace Registry.Domain.Events;

public record PetOwnerRemovedIntegrationEvent(Guid PetId, Guid OwnerId) : IDomainEvent;

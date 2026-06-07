using SharedKernel.Abstractions;

namespace Registry.Domain.Events;

public record PetOwnerAddedIntegrationEvent(Guid PetId, Guid OwnerId) : IDomainEvent;

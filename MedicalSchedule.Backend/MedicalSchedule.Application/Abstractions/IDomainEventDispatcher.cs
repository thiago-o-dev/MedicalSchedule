using MedicalSchedule.Domain.Abstractions;

namespace MedicalSchedule.Application.Abstractions;

public interface IDomainEventDispatcher
{
    Task DispatchAsync(IEnumerable<IDomainEvent> events, CancellationToken cancellationToken = default);
}

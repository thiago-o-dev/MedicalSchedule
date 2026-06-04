using MedicalSchedule.Application.Abstractions;
using MedicalSchedule.Domain.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace MedicalSchedule.Infrastructure.Auth;

public class DomainEventDispatcher(IServiceProvider serviceProvider) : IDomainEventDispatcher
{
    public async Task DispatchAsync(IEnumerable<IDomainEvent> events, CancellationToken cancellationToken = default)
    {
        foreach (var @event in events)
        {
            var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(@event.GetType());
            var handlers = serviceProvider.GetServices(handlerType);

            foreach (var handler in handlers)
            {
                var method = handlerType.GetMethod(nameof(IDomainEventHandler<IDomainEvent>.HandleAsync))!;
                await (Task)method.Invoke(handler, [@event, cancellationToken])!;
            }
        }
    }
}

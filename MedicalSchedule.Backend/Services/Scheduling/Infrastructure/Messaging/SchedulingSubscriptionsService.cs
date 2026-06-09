using BuildingBlocks.Messaging.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Scheduling.Contracts.Events;
using SharedKernel.Abstractions;

namespace Scheduling.Infrastructure.Messaging;

public sealed class SchedulingSubscriptionsService(
    IMessageSubscriber subscriber,
    IServiceProvider serviceProvider) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await subscriber.SubscribeAsync<PetDeletionRequestedEvent>(
            subscriptionId: "scheduling.pet-deletion-requested",
            handler: (ev, ct) => DispatchAsync(ev, ct),
            CancellationToken.None);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    private async Task DispatchAsync<TEvent>(TEvent @event, CancellationToken ct)
        where TEvent : class, IDomainEvent
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var handler = scope.ServiceProvider.GetRequiredService<IDomainEventHandler<TEvent>>();
        await handler.HandleAsync(@event, ct);
    }
}

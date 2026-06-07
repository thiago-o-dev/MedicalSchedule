using BuildingBlocks.Messaging.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Notifications.Contracts.Events;
using Notifications.Features;
using SharedKernel.Abstractions;

namespace Notifications.Infrastructure.Messaging;

public sealed class NotificationSubscriptionsService(
    IMessageSubscriber subscriber,
    IServiceProvider serviceProvider) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        // CancellationToken.None: subscriptions must live for the app lifetime,
        // not just the startup phase. The startup token is cancelled after startup completes.
        await subscriber.SubscribeAsync<ConsultationScheduledEvent>(
            subscriptionId: "notifications.consultation-scheduled",
            handler: (ev, ct) => DispatchAsync<ConsultationScheduledEvent>(ev, ct),
            CancellationToken.None);

        await subscriber.SubscribeAsync<ConsultationCancelledEvent>(
            subscriptionId: "notifications.consultation-cancelled",
            handler: (ev, ct) => DispatchAsync<ConsultationCancelledEvent>(ev, ct),
            CancellationToken.None);

        await subscriber.SubscribeAsync<ConsultationRescheduledEvent>(
            subscriptionId: "notifications.consultation-rescheduled",
            handler: (ev, ct) => DispatchAsync<ConsultationRescheduledEvent>(ev, ct),
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

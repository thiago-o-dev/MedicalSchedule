using BuildingBlocks.Messaging.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Scheduling.Contracts.Events;
using Scheduling.Domain.Enums;
using Scheduling.Infrastructure;
using SharedKernel.Abstractions;

namespace Scheduling.Features.Sagas;

public sealed class PetDeletionRequestedSagaHandler(
    ISchedulingUnitOfWork unitOfWork,
    IMessagePublisher publisher,
    ILogger<PetDeletionRequestedSagaHandler> logger) : IDomainEventHandler<PetDeletionRequestedEvent>
{
    public async Task HandleAsync(PetDeletionRequestedEvent @event, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        var futureConsultations = await unitOfWork.Consultations
            .Where(c =>
                c.PetId == @event.PetId &&
                c.Status == ConsultationStatus.Scheduled &&
                c.ScheduledAt > now)
            .CountAsync(cancellationToken);

        if (futureConsultations > 0)
        {
            var reason = $"Pet has {futureConsultations} future scheduled consultation(s). Cancel them before deletion.";

            logger.LogInformation(
                "Saga: rejecting deletion of pet {PetId} ({Count} future consultations).",
                @event.PetId, futureConsultations);

            await publisher.PublishAsync(
                new PetDeletionRejectedEvent(@event.PetId, reason),
                cancellationToken);
            return;
        }

        logger.LogInformation("Saga: approving deletion of pet {PetId}.", @event.PetId);

        await publisher.PublishAsync(
            new PetDeletionApprovedEvent(@event.PetId),
            cancellationToken);
    }
}

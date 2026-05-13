using MedicalSchedule.Domain.Abstractions;
using MedicalSchedule.Domain.Events.Consultations;
using Microsoft.Extensions.Logging;

namespace MedicalSchedule.Application.EventHandlers.Consultations;

public class ConsultationCancelledEventHandler(ILogger<ConsultationCancelledEventHandler> logger)
    : IDomainEventHandler<ConsultationCancelledEvent>
{
    public Task HandleAsync(ConsultationCancelledEvent @event, CancellationToken cancellationToken = default)
    {
        logger.LogInformation(
            "Consultation {Id} cancelled — Pet: {PetId}, Vet: {VetId}, Was scheduled at: {At}",
            @event.ConsultationId, @event.PetId, @event.VetId, @event.ScheduledAt);

        return Task.CompletedTask;
    }
}

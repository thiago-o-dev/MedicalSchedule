using MedicalSchedule.Domain.Abstractions;
using MedicalSchedule.Domain.Events.Consultations;
using Microsoft.Extensions.Logging;

namespace MedicalSchedule.Application.EventHandlers.Consultations;

public class ConsultationScheduledEventHandler(ILogger<ConsultationScheduledEventHandler> logger)
    : IDomainEventHandler<ConsultationScheduledEvent>
{
    public Task HandleAsync(ConsultationScheduledEvent @event, CancellationToken cancellationToken = default)
    {
        logger.LogInformation(
            "Consultation {Id} scheduled — Pet: {PetId}, Vet: {VetId}, At: {At}",
            @event.ConsultationId, @event.PetId, @event.VetId, @event.ScheduledAt);

        return Task.CompletedTask;
    }
}

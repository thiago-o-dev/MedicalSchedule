using Scheduling.Domain.Enums;
using Scheduling.Domain.Events;
using SharedKernel.Abstractions;
using SharedKernel.Exceptions;

namespace Scheduling.Domain.Entities;

public class Consultation : LifeCycleEntity
{
    // Maybe this on the future would be nice
    //IEnumerable<Guid> VetIds;
    //IEnumerable<Guid> PetIds;
    //IEnumerable<Guid> OwnerIds;

    public Guid VetId { get; private set; }
    public Guid PetId { get; private set; }

    public ConsultationStatus Status { get; private set; }

    public DateTime ScheduledAt { get; private set; }

    public string? Notes { get; private set; }

    public static Consultation Schedule(Guid petId, Guid vetId, DateTime scheduledAt, string? notes)
    {
        if (petId == Guid.Empty)
            throw new DomainValidationException("Pet is required.");
        if (vetId == Guid.Empty)
            throw new DomainValidationException("Vet is required.");
        if (scheduledAt <= DateTime.UtcNow)
            throw new DomainValidationException("Consultation must be scheduled for a future date and time.");

        var consultation = new Consultation
        {
            Id = Guid.NewGuid(),
            PetId = petId,
            VetId = vetId,
            ScheduledAt = scheduledAt,
            Status = ConsultationStatus.Scheduled,
            Notes = notes?.Trim(),
        };

        consultation.RaiseDomainEvent(
            new ConsultationScheduledEvent(consultation.Id, petId, vetId, scheduledAt));

        return consultation;
    }

    public void Cancel()
    {
        if (Status != ConsultationStatus.Scheduled)
            throw new DomainValidationException("Only scheduled consultations can be cancelled.");

        Status = ConsultationStatus.Cancelled;
        Touch();

        RaiseDomainEvent(new ConsultationCancelledEvent(Id, PetId, VetId, ScheduledAt));
    }

    public void Reschedule(DateTime newScheduledAt)
    {
        if (Status != ConsultationStatus.Scheduled)
            throw new DomainValidationException("Only scheduled consultations can be rescheduled.");
        if (newScheduledAt <= DateTime.UtcNow)
            throw new DomainValidationException("Consultation must be rescheduled for a future date and time.");

        var previous = ScheduledAt;
        ScheduledAt = newScheduledAt;
        Touch();

        RaiseDomainEvent(new ConsultationRescheduledEvent(Id, PetId, VetId, previous, newScheduledAt));
    }
}

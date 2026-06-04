using MedicalSchedule.Domain.Abstractions;
using MedicalSchedule.Domain.Enums;
using MedicalSchedule.Domain.Events.Consultations;
using MedicalSchedule.Domain.Exceptions;

namespace MedicalSchedule.Domain.Entities.Consultations;

public class Consultation : LifeCycleEntity
{
    public Guid PetId { get; private set; }
    public Guid VetId { get; private set; }
    public DateTime ScheduledAt { get; private set; }
    public ConsultationStatus Status { get; private set; }
    public string? Notes { get; private set; }

    private Consultation() { }

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
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
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

    public void Complete(string? notes = null)
    {
        if (Status != ConsultationStatus.Scheduled)
            throw new DomainValidationException("Only scheduled consultations can be completed.");

        Status = ConsultationStatus.Completed;
        if (notes is not null) Notes = notes.Trim();
        Touch();
    }

    public bool IsScheduledInFuture()
        => Status == ConsultationStatus.Scheduled && ScheduledAt > DateTime.UtcNow;
}

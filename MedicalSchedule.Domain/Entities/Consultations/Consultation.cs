using MedicalSchedule.Domain.Abstractions;
using MedicalSchedule.Domain.Enums;
using MedicalSchedule.Domain.Events.Consultations;
using MedicalSchedule.Domain.Exceptions;

namespace MedicalSchedule.Domain.Entities.Consultations;

public class Consultation : LifeCycleEntity
{
    public const int MinDurationMinutes = 15;
    public const int MaxDurationMinutes = 240;
    public const int BusinessHoursStart = 8;
    public const int BusinessHoursEnd = 18;
    public const int MinHoursToCancel = 24;

    public Guid PetId { get; private set; }
    public Guid VetId { get; private set; }
    public DateTime ScheduledAt { get; private set; }
    public int DurationMinutes { get; private set; }
    public DateTime EndsAt => ScheduledAt.AddMinutes(DurationMinutes);
    public ConsultationStatus Status { get; private set; }
    public string? Notes { get; private set; }

    private Consultation() { }

    public static Consultation Schedule(Guid petId, Guid vetId, DateTime scheduledAt, int durationMinutes, string? notes)
    {
        if (petId == Guid.Empty)
            throw new DomainValidationException("Pet is required.");
        if (vetId == Guid.Empty)
            throw new DomainValidationException("Vet is required.");
        if (scheduledAt <= DateTime.UtcNow)
            throw new DomainValidationException("Consultation must be scheduled for a future date and time.");
        if (durationMinutes < MinDurationMinutes || durationMinutes > MaxDurationMinutes)
            throw new DomainValidationException($"Duration must be between {MinDurationMinutes} and {MaxDurationMinutes} minutes.");
        if (scheduledAt.DayOfWeek == DayOfWeek.Sunday)
            throw new DomainValidationException("Consultations cannot be scheduled on Sundays.");
        if (scheduledAt.Hour < BusinessHoursStart || scheduledAt.Hour >= BusinessHoursEnd)
            throw new DomainValidationException($"Consultations must be scheduled between {BusinessHoursStart}:00 and {BusinessHoursEnd}:00.");

        var consultation = new Consultation
        {
            Id = Guid.NewGuid(),
            PetId = petId,
            VetId = vetId,
            ScheduledAt = scheduledAt,
            DurationMinutes = durationMinutes,
            Status = ConsultationStatus.Scheduled,
            Notes = notes?.Trim(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        consultation.RaiseDomainEvent(
            new ConsultationScheduledEvent(consultation.Id, petId, vetId, scheduledAt));

        return consultation;
    }

    public void Reschedule(DateTime newScheduledAt, int newDurationMinutes)
    {
        if (Status != ConsultationStatus.Scheduled)
            throw new DomainValidationException("Only scheduled consultations can be rescheduled.");
        if (newScheduledAt <= DateTime.UtcNow)
            throw new DomainValidationException("New date must be in the future.");
        if (newDurationMinutes < MinDurationMinutes || newDurationMinutes > MaxDurationMinutes)
            throw new DomainValidationException($"Duration must be between {MinDurationMinutes} and {MaxDurationMinutes} minutes.");
        if (newScheduledAt.DayOfWeek == DayOfWeek.Sunday)
            throw new DomainValidationException("Consultations cannot be scheduled on Sundays.");
        if (newScheduledAt.Hour < BusinessHoursStart || newScheduledAt.Hour >= BusinessHoursEnd)
            throw new DomainValidationException($"Consultations must be scheduled between {BusinessHoursStart}:00 and {BusinessHoursEnd}:00.");

        var previous = ScheduledAt;
        ScheduledAt = newScheduledAt;
        DurationMinutes = newDurationMinutes;
        Touch();

        RaiseDomainEvent(new ConsultationRescheduledEvent(Id, PetId, VetId, previous, newScheduledAt));
    }

    public void Cancel()
    {
        if (Status != ConsultationStatus.Scheduled)
            throw new DomainValidationException("Only scheduled consultations can be cancelled.");
        if (ScheduledAt - DateTime.UtcNow < TimeSpan.FromHours(MinHoursToCancel))
            throw new DomainValidationException($"Consultations cannot be cancelled less than {MinHoursToCancel} hours in advance.");

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

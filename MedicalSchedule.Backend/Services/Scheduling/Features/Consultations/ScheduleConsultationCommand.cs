namespace Scheduling.Features.Consultations;

public record ScheduleConsultationCommand(
    Guid PetId,
    Guid VetId,
    DateTime ScheduledAt,
    string? Notes);

namespace Scheduling.Features.Consultations;

public record ScheduleConsultationCommand(
    Guid PetId,
    Guid VetId,
    Guid OwnerId,
    DateTime ScheduledAt,
    string? Notes);

namespace Scheduling.Api.Requests;

public record ScheduleConsultationRequest(
    Guid PetId,
    Guid VetId,
    DateTime ScheduledAt,
    string? Notes);

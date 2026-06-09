namespace Scheduling.Api.Requests;

public record ScheduleConsultationRequest(
    Guid PetId,
    Guid VetId,
    Guid OwnerId,
    DateTime ScheduledAt,
    string? Notes);

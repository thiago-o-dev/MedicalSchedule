using Scheduling.Domain.Enums;

namespace Scheduling.Features.Consultations;

public record ConsultationResponse(
    Guid Id,
    Guid PetId,
    Guid VetId,
    Guid OwnerId,
    ConsultationStatus Status,
    DateTime ScheduledAt,
    string? Notes);

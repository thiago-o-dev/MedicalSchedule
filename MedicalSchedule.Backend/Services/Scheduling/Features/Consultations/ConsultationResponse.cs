using Scheduling.Domain.Enums;

namespace Scheduling.Features.Consultations;

public record ConsultationResponse(
    Guid Id,
    Guid PetId,
    Guid VetId,
    ConsultationStatus Status,
    DateTime ScheduledAt,
    string? Notes);

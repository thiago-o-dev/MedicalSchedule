using Scheduling.Domain.Enums;

namespace Scheduling.Features.Consultations;

public record GetAllConsultationsQuery(
    Guid? PetId = null,
    Guid? VetId = null,
    ConsultationStatus? Status = null);

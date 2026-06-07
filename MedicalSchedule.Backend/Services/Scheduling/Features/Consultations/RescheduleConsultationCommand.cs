namespace Scheduling.Features.Consultations;

public record RescheduleConsultationCommand(Guid ConsultationId, DateTime NewScheduledAt);

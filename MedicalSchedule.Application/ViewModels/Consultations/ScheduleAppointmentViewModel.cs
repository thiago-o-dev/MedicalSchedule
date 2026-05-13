namespace MedicalSchedule.Application.ViewModels.Consultations;

public record ScheduleAppointmentViewModel(Guid PetId, Guid VetId, DateTime ScheduledAt, string? Notes);

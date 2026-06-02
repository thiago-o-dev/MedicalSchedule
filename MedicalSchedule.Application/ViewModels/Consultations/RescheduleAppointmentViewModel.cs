namespace MedicalSchedule.Application.ViewModels.Consultations;

public record RescheduleAppointmentViewModel(DateTime NewScheduledAt, int NewDurationMinutes);

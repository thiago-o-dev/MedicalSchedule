using FluentValidation;
using MedicalSchedule.Application.ViewModels.Consultations;
using MedicalSchedule.Domain.Entities.Consultations;

namespace MedicalSchedule.Application.Validators.Consultations;

public class RescheduleAppointmentViewModelValidator : AbstractValidator<RescheduleAppointmentViewModel>
{
    public RescheduleAppointmentViewModelValidator()
    {
        RuleFor(x => x.NewScheduledAt)
            .GreaterThan(DateTime.UtcNow)
            .WithMessage("New scheduled date must be in the future.");

        RuleFor(x => x.NewDurationMinutes)
            .InclusiveBetween(Consultation.MinDurationMinutes, Consultation.MaxDurationMinutes)
            .WithMessage($"Duration must be between {Consultation.MinDurationMinutes} and {Consultation.MaxDurationMinutes} minutes.");
    }
}

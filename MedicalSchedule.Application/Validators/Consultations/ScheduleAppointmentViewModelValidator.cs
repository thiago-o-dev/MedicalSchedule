using FluentValidation;
using MedicalSchedule.Application.ViewModels.Consultations;
using MedicalSchedule.Domain.Entities.Consultations;

namespace MedicalSchedule.Application.Validators.Consultations;

public class ScheduleAppointmentViewModelValidator : AbstractValidator<ScheduleAppointmentViewModel>
{
    public ScheduleAppointmentViewModelValidator()
    {
        RuleFor(x => x.PetId).NotEmpty();
        RuleFor(x => x.VetId).NotEmpty();
        RuleFor(x => x.ScheduledAt).GreaterThan(DateTime.UtcNow)
            .WithMessage("Scheduled date must be in the future.");
        RuleFor(x => x.DurationMinutes)
            .InclusiveBetween(Consultation.MinDurationMinutes, Consultation.MaxDurationMinutes)
            .WithMessage($"Duration must be between {Consultation.MinDurationMinutes} and {Consultation.MaxDurationMinutes} minutes.");
        RuleFor(x => x.Notes).MaximumLength(1000).When(x => x.Notes is not null);
    }
}

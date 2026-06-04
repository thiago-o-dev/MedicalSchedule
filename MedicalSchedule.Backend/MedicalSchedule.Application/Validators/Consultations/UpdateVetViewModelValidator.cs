using FluentValidation;
using MedicalSchedule.Application.ViewModels.Consultations;

namespace MedicalSchedule.Application.Validators.Consultations;

public class UpdateVetViewModelValidator : AbstractValidator<UpdateVetViewModel>
{
    public UpdateVetViewModelValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Specialty).NotEmpty().MaximumLength(100);
    }
}

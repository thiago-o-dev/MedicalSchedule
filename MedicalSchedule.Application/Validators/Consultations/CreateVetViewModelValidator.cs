using FluentValidation;
using MedicalSchedule.Application.ViewModels.Consultations;

namespace MedicalSchedule.Application.Validators.Consultations;

public class CreateVetViewModelValidator : AbstractValidator<CreateVetViewModel>
{
    public CreateVetViewModelValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Crm).NotEmpty().MaximumLength(20);
        RuleFor(x => x.Specialty).NotEmpty().MaximumLength(100);
    }
}

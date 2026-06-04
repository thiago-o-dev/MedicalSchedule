using FluentValidation;
using MedicalSchedule.Application.ViewModels.Registration;

namespace MedicalSchedule.Application.Validators.Registration;

public class UpdatePetViewModelValidator : AbstractValidator<UpdatePetViewModel>
{
    public UpdatePetViewModelValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Species).IsInEnum();
        RuleFor(x => x.Breed).NotEmpty().MaximumLength(100);
        RuleFor(x => x.BirthDate).LessThan(DateOnly.FromDateTime(DateTime.Today))
            .WithMessage("Birth date must be in the past.");
    }
}

using FluentValidation;
using MedicalSchedule.Application.ViewModels.Registration;

namespace MedicalSchedule.Application.Validators.Registration;

public class CreateOwnerViewModelValidator : AbstractValidator<CreateOwnerViewModel>
{
    public CreateOwnerViewModelValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Cpf).NotEmpty().Length(11).Matches(@"^\d{11}$")
            .WithMessage("CPF must contain exactly 11 digits.");
        RuleFor(x => x.Phone).NotEmpty().MaximumLength(20);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(200);
    }
}

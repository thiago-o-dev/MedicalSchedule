using FluentValidation;
using MedicalSchedule.Application.ViewModels.Registration;

namespace MedicalSchedule.Application.Validators.Registration;

public class UpdateOwnerViewModelValidator : AbstractValidator<UpdateOwnerViewModel>
{
    public UpdateOwnerViewModelValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Phone).NotEmpty().MaximumLength(20);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(200);
    }
}

using FluentValidation;
using MedicalSchedule.Application.Abstractions;
using MedicalSchedule.Application.Exceptions;
using MedicalSchedule.Application.ViewModels.Registration;
using Microsoft.EntityFrameworkCore;

namespace MedicalSchedule.Application.Features.Registration.Pets;

public class UpdatePetCommandHandler(
    IValidator<UpdatePetViewModel> validator,
    IUnitOfWork unitOfWork)
{
    public async Task<PetViewModel> HandleAsync(
        Guid id,
        UpdatePetViewModel vm,
        CancellationToken cancellationToken = default)
    {
        var validation = await validator.ValidateAsync(vm, cancellationToken);
        if (!validation.IsValid)
            throw new BusinessLogicException(string.Join(" ", validation.Errors.Select(e => e.ErrorMessage)));

        var pet = await unitOfWork.Pets.FirstOrDefaultAsync(p => p.Id == id, cancellationToken)
            ?? throw new NotFoundException($"Pet '{id}' not found.");

        pet.Update(vm.Name, vm.Species, vm.Breed, vm.BirthDate);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return PetViewModel.From(pet);
    }
}

using FluentValidation;
using MedicalSchedule.Application.Abstractions;
using MedicalSchedule.Application.Exceptions;
using MedicalSchedule.Application.ViewModels.Registration;
using MedicalSchedule.Domain.Entities.Registration;
using Microsoft.EntityFrameworkCore;

namespace MedicalSchedule.Application.Features.Registration.Pets;

public class CreatePetCommandHandler(
    IValidator<CreatePetViewModel> validator,
    IUnitOfWork unitOfWork)
{
    public async Task<PetViewModel> HandleAsync(
        CreatePetViewModel vm,
        CancellationToken cancellationToken = default)
    {
        var validation = await validator.ValidateAsync(vm, cancellationToken);
        if (!validation.IsValid)
            throw new BusinessLogicException(string.Join(" ", validation.Errors.Select(e => e.ErrorMessage)));

        var ownerExists = await unitOfWork.Owners
            .AnyAsync(o => o.Id == vm.OwnerId && o.IsActive, cancellationToken);
        if (!ownerExists)
            throw new NotFoundException($"Owner '{vm.OwnerId}' not found.");

        var pet = Pet.Create(vm.Name, vm.Species, vm.Breed, vm.BirthDate, vm.OwnerId);

        unitOfWork.Pets.Add(pet);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return PetViewModel.From(pet);
    }
}

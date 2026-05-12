using FluentValidation;
using MedicalSchedule.Application.Abstractions;
using MedicalSchedule.Application.Exceptions;
using MedicalSchedule.Application.ViewModels.Consultations;
using Microsoft.EntityFrameworkCore;

namespace MedicalSchedule.Application.Features.Consultations.Vets;

public class UpdateVetCommandHandler(
    IValidator<UpdateVetViewModel> validator,
    IUnitOfWork unitOfWork)
{
    public async Task<VetViewModel> HandleAsync(
        Guid id,
        UpdateVetViewModel vm,
        CancellationToken cancellationToken = default)
    {
        var validation = await validator.ValidateAsync(vm, cancellationToken);
        if (!validation.IsValid)
            throw new BusinessLogicException(string.Join(" ", validation.Errors.Select(e => e.ErrorMessage)));

        var vet = await unitOfWork.Vets.FirstOrDefaultAsync(v => v.Id == id, cancellationToken)
            ?? throw new NotFoundException($"Vet '{id}' not found.");

        vet.Update(vm.Name, vm.Specialty);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return VetViewModel.From(vet);
    }
}

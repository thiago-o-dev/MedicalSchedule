using FluentValidation;
using MedicalSchedule.Application.Abstractions;
using MedicalSchedule.Application.Exceptions;
using MedicalSchedule.Application.ViewModels.Consultations;
using MedicalSchedule.Domain.Entities.Consultations;
using Microsoft.EntityFrameworkCore;

namespace MedicalSchedule.Application.Features.Consultations.Vets;

public class CreateVetCommandHandler(
    IValidator<CreateVetViewModel> validator,
    IUnitOfWork unitOfWork)
{
    public async Task<VetViewModel> HandleAsync(
        CreateVetViewModel vm,
        CancellationToken cancellationToken = default)
    {
        var validation = await validator.ValidateAsync(vm, cancellationToken);
        if (!validation.IsValid)
            throw new BusinessLogicException(string.Join(" ", validation.Errors.Select(e => e.ErrorMessage)));

        var crmExists = await unitOfWork.Vets
            .AnyAsync(v => v.Crm == vm.Crm.Trim().ToUpperInvariant(), cancellationToken);
        if (crmExists)
            throw new ConflictException($"CRM '{vm.Crm}' is already registered.");

        var vet = Vet.Create(vm.Name, vm.Crm, vm.Specialty);

        unitOfWork.Vets.Add(vet);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return VetViewModel.From(vet);
    }
}

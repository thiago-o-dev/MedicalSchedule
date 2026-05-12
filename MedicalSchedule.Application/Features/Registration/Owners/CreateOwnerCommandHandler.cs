using FluentValidation;
using MedicalSchedule.Application.Abstractions;
using MedicalSchedule.Application.Exceptions;
using MedicalSchedule.Application.ViewModels.Registration;
using MedicalSchedule.Domain.Entities.Registration;
using Microsoft.EntityFrameworkCore;

namespace MedicalSchedule.Application.Features.Registration.Owners;

public class CreateOwnerCommandHandler(
    IValidator<CreateOwnerViewModel> validator,
    IUnitOfWork unitOfWork)
{
    public async Task<OwnerViewModel> HandleAsync(
        CreateOwnerViewModel vm,
        CancellationToken cancellationToken = default)
    {
        var validation = await validator.ValidateAsync(vm, cancellationToken);
        if (!validation.IsValid)
            throw new BusinessLogicException(string.Join(" ", validation.Errors.Select(e => e.ErrorMessage)));

        var cpfExists = await unitOfWork.Owners
            .AnyAsync(o => o.Cpf == vm.Cpf.Trim(), cancellationToken);
        if (cpfExists)
            throw new ConflictException($"CPF '{vm.Cpf}' is already registered.");

        var owner = Owner.Create(vm.Name, vm.Cpf, vm.Phone, vm.Email);

        unitOfWork.Owners.Add(owner);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return OwnerViewModel.From(owner);
    }
}

using FluentValidation;
using MedicalSchedule.Application.Abstractions;
using MedicalSchedule.Application.Exceptions;
using MedicalSchedule.Application.ViewModels.Registration;
using Microsoft.EntityFrameworkCore;

namespace MedicalSchedule.Application.Features.Registration.Owners;

public class UpdateOwnerCommandHandler(
    IValidator<UpdateOwnerViewModel> validator,
    IUnitOfWork unitOfWork)
{
    public async Task<OwnerViewModel> HandleAsync(
        Guid id,
        UpdateOwnerViewModel vm,
        CancellationToken cancellationToken = default)
    {
        var validation = await validator.ValidateAsync(vm, cancellationToken);
        if (!validation.IsValid)
            throw new BusinessLogicException(string.Join(" ", validation.Errors.Select(e => e.ErrorMessage)));

        var owner = await unitOfWork.Owners.FirstOrDefaultAsync(o => o.Id == id, cancellationToken)
            ?? throw new NotFoundException($"Owner '{id}' not found.");

        owner.Update(vm.Name, vm.Phone, vm.Email);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return OwnerViewModel.From(owner);
    }
}

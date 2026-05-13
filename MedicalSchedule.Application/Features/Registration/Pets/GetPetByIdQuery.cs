using MedicalSchedule.Application.Abstractions;
using MedicalSchedule.Application.Exceptions;
using MedicalSchedule.Application.ViewModels.Registration;
using Microsoft.EntityFrameworkCore;

namespace MedicalSchedule.Application.Features.Registration.Pets;

public class GetPetByIdQuery(IUnitOfWork unitOfWork)
{
    public async Task<PetViewModel> HandleAsync(
        Guid id,
        CancellationToken cancellationToken = default)
        => await unitOfWork.Pets
            .Where(p => p.Id == id)
            .Select(PetViewModel.FromEntity)
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException($"Pet '{id}' not found.");
}

using MedicalSchedule.Application.Abstractions;
using MedicalSchedule.Application.ViewModels.Registration;
using Microsoft.EntityFrameworkCore;

namespace MedicalSchedule.Application.Features.Registration.Pets;

public class GetAllPetsQuery(IUnitOfWork unitOfWork)
{
    public async Task<IReadOnlyList<PetViewModel>> HandleAsync(
        Guid? ownerId = null,
        CancellationToken cancellationToken = default)
        => await unitOfWork.Pets
            .Where(p => p.IsActive)
            .Where(p => ownerId == null || p.OwnerId == ownerId)
            .Select(PetViewModel.FromEntity)
            .ToListAsync(cancellationToken);
}

using MedicalSchedule.Application.Abstractions;
using MedicalSchedule.Application.ViewModels.Consultations;
using Microsoft.EntityFrameworkCore;

namespace MedicalSchedule.Application.Features.Consultations.Vets;

public class GetAllVetsQuery(IUnitOfWork unitOfWork)
{
    public async Task<IReadOnlyList<VetViewModel>> HandleAsync(
        CancellationToken cancellationToken = default)
        => await unitOfWork.Vets
            .Where(v => v.IsActive)
            .Select(VetViewModel.FromEntity)
            .ToListAsync(cancellationToken);
}

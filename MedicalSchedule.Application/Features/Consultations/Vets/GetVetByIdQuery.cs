using MedicalSchedule.Application.Abstractions;
using MedicalSchedule.Application.Exceptions;
using MedicalSchedule.Application.ViewModels.Consultations;
using Microsoft.EntityFrameworkCore;

namespace MedicalSchedule.Application.Features.Consultations.Vets;

public class GetVetByIdQuery(IUnitOfWork unitOfWork)
{
    public async Task<VetViewModel> HandleAsync(
        Guid id,
        CancellationToken cancellationToken = default)
        => await unitOfWork.Vets
            .Where(v => v.Id == id)
            .Select(VetViewModel.FromEntity)
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException($"Vet '{id}' not found.");
}

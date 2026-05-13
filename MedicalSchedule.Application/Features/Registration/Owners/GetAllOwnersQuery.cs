using MedicalSchedule.Application.Abstractions;
using MedicalSchedule.Application.ViewModels.Registration;
using Microsoft.EntityFrameworkCore;

namespace MedicalSchedule.Application.Features.Registration.Owners;

public class GetAllOwnersQuery(IUnitOfWork unitOfWork)
{
    public async Task<IReadOnlyList<OwnerViewModel>> HandleAsync(
        CancellationToken cancellationToken = default)
        => await unitOfWork.Owners
            .Where(o => o.IsActive)
            .Select(OwnerViewModel.FromEntity)
            .ToListAsync(cancellationToken);
}

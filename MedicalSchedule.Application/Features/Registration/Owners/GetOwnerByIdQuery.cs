using MedicalSchedule.Application.Abstractions;
using MedicalSchedule.Application.Exceptions;
using MedicalSchedule.Application.ViewModels.Registration;
using Microsoft.EntityFrameworkCore;

namespace MedicalSchedule.Application.Features.Registration.Owners;

public class GetOwnerByIdQuery(IUnitOfWork unitOfWork)
{
    public async Task<OwnerViewModel> HandleAsync(
        Guid id,
        CancellationToken cancellationToken = default)
        => await unitOfWork.Owners
            .Where(o => o.Id == id)
            .Select(OwnerViewModel.FromEntity)
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException($"Owner '{id}' not found.");
}

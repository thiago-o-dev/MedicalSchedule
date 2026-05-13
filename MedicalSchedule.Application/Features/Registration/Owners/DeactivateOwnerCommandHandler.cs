using MedicalSchedule.Application.Abstractions;
using MedicalSchedule.Application.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace MedicalSchedule.Application.Features.Registration.Owners;

public class DeactivateOwnerCommandHandler(IUnitOfWork unitOfWork)
{
    public async Task HandleAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var owner = await unitOfWork.Owners.FirstOrDefaultAsync(o => o.Id == id, cancellationToken)
            ?? throw new NotFoundException($"Owner '{id}' not found.");

        var hasActivePets = await unitOfWork.Pets
            .AnyAsync(p => p.OwnerId == id && p.IsActive, cancellationToken);
        if (hasActivePets)
            throw new BusinessLogicException("Cannot deactivate an owner with active pets.");

        owner.Deactivate();
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

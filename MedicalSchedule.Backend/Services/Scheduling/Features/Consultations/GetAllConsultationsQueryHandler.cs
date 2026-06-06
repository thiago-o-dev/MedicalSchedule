using Microsoft.EntityFrameworkCore;
using Scheduling.Infrastructure;
using SharedKernel.Abstractions;

namespace Scheduling.Features.Consultations;

public sealed class GetAllConsultationsQueryHandler(ISchedulingUnitOfWork unitOfWork)
    : IQueryHandler<GetAllConsultationsQuery, IReadOnlyList<ConsultationResponse>>
{
    public async Task<IReadOnlyList<ConsultationResponse>> HandleAsync(
        GetAllConsultationsQuery query,
        CancellationToken cancellationToken = default)
        => await unitOfWork.Consultations
            .Where(c => query.PetId == null || c.PetId == query.PetId)
            .Where(c => query.VetId == null || c.VetId == query.VetId)
            .Where(c => query.Status == null || c.Status == query.Status)
            .OrderBy(c => c.ScheduledAt)
            .Select(c => new ConsultationResponse(c.Id, c.PetId, c.VetId, c.Status, c.ScheduledAt, c.Notes))
            .ToListAsync(cancellationToken);
}

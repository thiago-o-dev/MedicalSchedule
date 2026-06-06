using Microsoft.EntityFrameworkCore;
using Scheduling.Infrastructure;
using SharedKernel.Abstractions;
using SharedKernel.Exceptions;

namespace Scheduling.Features.Consultations;

public sealed class GetConsultationByIdQueryHandler(ISchedulingUnitOfWork unitOfWork)
    : IQueryHandler<GetConsultationByIdQuery, ConsultationResponse>
{
    public async Task<ConsultationResponse> HandleAsync(
        GetConsultationByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var c = await unitOfWork.Consultations
            .FirstOrDefaultAsync(x => x.Id == query.ConsultationId, cancellationToken)
            ?? throw new NotFoundException($"Consultation '{query.ConsultationId}' not found.");

        return new ConsultationResponse(c.Id, c.PetId, c.VetId, c.Status, c.ScheduledAt, c.Notes);
    }
}

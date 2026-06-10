using Microsoft.EntityFrameworkCore;
using Scheduling.Infrastructure;
using SharedKernel.Abstractions;
using SharedKernel.Exceptions;

namespace Scheduling.Features.Consultations;

public sealed class CompleteConsultationByIdCommandHandler(ISchedulingUnitOfWork unitOfWork)
    : ICommandHandler<CompleteConsultationByIdCommand>
{
    public async Task HandleAsync(CompleteConsultationByIdCommand command, CancellationToken cancellationToken = default)
    {
        var consultation = await unitOfWork.Consultations
            .FirstOrDefaultAsync(c => c.Id == command.ConsultationId, cancellationToken)
            ?? throw new NotFoundException($"Consultation '{command.ConsultationId}' not found.");

        consultation.Complete();
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

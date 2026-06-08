using Microsoft.EntityFrameworkCore;
using Scheduling.Infrastructure;
using SharedKernel.Abstractions;
using SharedKernel.Exceptions;

namespace Scheduling.Features.Consultations;

public sealed class RescheduleConsultationCommandHandler(ISchedulingUnitOfWork unitOfWork)
    : ICommandHandler<RescheduleConsultationCommand>
{
    public async Task HandleAsync(RescheduleConsultationCommand command, CancellationToken cancellationToken = default)
    {
        var consultation = await unitOfWork.Consultations
            .FirstOrDefaultAsync(c => c.Id == command.ConsultationId, cancellationToken)
            ?? throw new NotFoundException($"Consultation '{command.ConsultationId}' not found.");

        consultation.Reschedule(DateTime.SpecifyKind(command.NewScheduledAt, DateTimeKind.Utc));
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

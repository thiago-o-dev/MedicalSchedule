using Microsoft.EntityFrameworkCore;
using Scheduling.Domain.Entities;
using Scheduling.Domain.Enums;
using Scheduling.Infrastructure;
using SharedKernel.Abstractions;
using SharedKernel.Exceptions;

namespace Scheduling.Features.Consultations;

public sealed class ScheduleConsultationCommandHandler(ISchedulingUnitOfWork unitOfWork)
    : ICommandHandler<ScheduleConsultationCommand, Guid>
{
    public async Task<Guid> HandleAsync(ScheduleConsultationCommand command, CancellationToken cancellationToken = default)
    {
        var conflictExists = await unitOfWork.Consultations
            .AnyAsync(c =>
                c.VetId == command.VetId &&
                c.Status == ConsultationStatus.Scheduled &&
                c.ScheduledAt == command.ScheduledAt,
                cancellationToken);

        if (conflictExists)
            throw new ConflictException("This vet already has a consultation scheduled at the specified time.");

        var consultation = Consultation.Schedule(command.PetId, command.VetId, command.ScheduledAt, command.Notes);

        unitOfWork.Consultations.Add(consultation);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return consultation.Id;
    }
}

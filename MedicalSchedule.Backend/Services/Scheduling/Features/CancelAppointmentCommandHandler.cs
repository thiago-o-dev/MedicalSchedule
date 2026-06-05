using SharedKernel.Abstractions;
using SharedKernel.Exceptions;
using MedicalSchedule.Application.ViewModels.Consultations;

namespace Scheduling.Features;

public class CancelAppointmentCommandHandler(IUnitOfWork unitOfWork)
{
    public async Task<AppointmentViewModel> HandleAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var consultation = await unitOfWork.Consultations
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken)
            ?? throw new NotFoundException($"Appointment '{id}' not found.");

        consultation.Cancel();
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return AppointmentViewModel.From(consultation);
    }
}

using MedicalSchedule.Application.Abstractions;
using MedicalSchedule.Application.Exceptions;
using MedicalSchedule.Application.ViewModels.Consultations;
using Microsoft.EntityFrameworkCore;

namespace MedicalSchedule.Application.Features.Consultations.Appointments;

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

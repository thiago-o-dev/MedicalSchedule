using MedicalSchedule.Application.Abstractions;
using MedicalSchedule.Application.Exceptions;
using MedicalSchedule.Application.ViewModels.Consultations;
using Microsoft.EntityFrameworkCore;

namespace Scheduling.Features;

public class GetAppointmentByIdQuery(IUnitOfWork unitOfWork)
{
    public async Task<AppointmentViewModel> HandleAsync(
        Guid id,
        CancellationToken cancellationToken = default)
        => await unitOfWork.Consultations
            .Where(c => c.Id == id)
            .Select(AppointmentViewModel.FromEntity)
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException($"Appointment '{id}' not found.");
}

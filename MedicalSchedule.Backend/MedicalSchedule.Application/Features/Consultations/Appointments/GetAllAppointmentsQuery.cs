using MedicalSchedule.Application.Abstractions;
using MedicalSchedule.Application.ViewModels.Consultations;
using MedicalSchedule.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace MedicalSchedule.Application.Features.Consultations.Appointments;

public class GetAllAppointmentsQuery(IUnitOfWork unitOfWork)
{
    public async Task<IReadOnlyList<AppointmentViewModel>> HandleAsync(
        Guid? petId = null,
        Guid? vetId = null,
        ConsultationStatus? status = null,
        CancellationToken cancellationToken = default)
        => await unitOfWork.Consultations
            .Where(c => petId == null || c.PetId == petId)
            .Where(c => vetId == null || c.VetId == vetId)
            .Where(c => status == null || c.Status == status)
            .OrderBy(c => c.ScheduledAt)
            .Select(AppointmentViewModel.FromEntity)
            .ToListAsync(cancellationToken);
}

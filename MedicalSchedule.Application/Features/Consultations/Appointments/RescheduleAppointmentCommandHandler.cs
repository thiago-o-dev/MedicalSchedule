using FluentValidation;
using MedicalSchedule.Application.Abstractions;
using MedicalSchedule.Application.Exceptions;
using MedicalSchedule.Application.ViewModels.Consultations;
using MedicalSchedule.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace MedicalSchedule.Application.Features.Consultations.Appointments;

public class RescheduleAppointmentCommandHandler(
    IValidator<RescheduleAppointmentViewModel> validator,
    IUnitOfWork unitOfWork)
{
    public async Task<AppointmentViewModel> HandleAsync(
        Guid id,
        RescheduleAppointmentViewModel vm,
        CancellationToken cancellationToken = default)
    {
        var validation = await validator.ValidateAsync(vm, cancellationToken);
        if (!validation.IsValid)
            throw new BusinessLogicException(string.Join(" ", validation.Errors.Select(e => e.ErrorMessage)));

        var consultation = await unitOfWork.Consultations
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken)
            ?? throw new NotFoundException($"Appointment '{id}' not found.");

        var newEndsAt = vm.NewScheduledAt.AddMinutes(vm.NewDurationMinutes);

        var vetConflict = await unitOfWork.Consultations
            .AnyAsync(c =>
                c.Id != id &&
                c.VetId == consultation.VetId &&
                c.Status == ConsultationStatus.Scheduled &&
                c.ScheduledAt < newEndsAt &&
                vm.NewScheduledAt < c.ScheduledAt.AddMinutes(c.DurationMinutes),
                cancellationToken);
        if (vetConflict)
            throw new ConflictException("This vet already has a consultation overlapping the requested time slot.");

        var petConflict = await unitOfWork.Consultations
            .AnyAsync(c =>
                c.Id != id &&
                c.PetId == consultation.PetId &&
                c.Status == ConsultationStatus.Scheduled &&
                c.ScheduledAt < newEndsAt &&
                vm.NewScheduledAt < c.ScheduledAt.AddMinutes(c.DurationMinutes),
                cancellationToken);
        if (petConflict)
            throw new ConflictException("This pet already has a consultation overlapping the requested time slot.");

        consultation.Reschedule(vm.NewScheduledAt, vm.NewDurationMinutes);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return AppointmentViewModel.From(consultation);
    }
}

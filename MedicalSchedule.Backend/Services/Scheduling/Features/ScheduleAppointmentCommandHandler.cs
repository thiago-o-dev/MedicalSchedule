using FluentValidation;
using MedicalSchedule.Application.Abstractions;
using MedicalSchedule.Application.Exceptions;
using MedicalSchedule.Application.ViewModels.Consultations;
using MedicalSchedule.Domain.Entities.Consultations;
using MedicalSchedule.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Scheduling.Features;

public class ScheduleAppointmentCommandHandler(
    IValidator<ScheduleAppointmentViewModel> validator,
    IUnitOfWork unitOfWork)
{
    public async Task<AppointmentViewModel> HandleAsync(
        ScheduleAppointmentViewModel vm,
        CancellationToken cancellationToken = default)
    {
        var validation = await validator.ValidateAsync(vm, cancellationToken);
        if (!validation.IsValid)
            throw new BusinessLogicException(string.Join(" ", validation.Errors.Select(e => e.ErrorMessage)));

        var petExists = await unitOfWork.Pets
            .AnyAsync(p => p.Id == vm.PetId && p.IsActive, cancellationToken);
        if (!petExists)
            throw new NotFoundException($"Pet '{vm.PetId}' not found.");

        var vetExists = await unitOfWork.Vets
            .AnyAsync(v => v.Id == vm.VetId && v.IsActive, cancellationToken);
        if (!vetExists)
            throw new NotFoundException($"Vet '{vm.VetId}' not found.");

        var conflictExists = await unitOfWork.Consultations
            .AnyAsync(c =>
                c.VetId == vm.VetId &&
                c.Status == ConsultationStatus.Scheduled &&
                c.ScheduledAt == vm.ScheduledAt,
                cancellationToken);
        if (conflictExists)
            throw new ConflictException("This vet already has a consultation scheduled at the specified time.");

        var consultation = Consultation.Schedule(vm.PetId, vm.VetId, vm.ScheduledAt, vm.Notes);

        unitOfWork.Consultations.Add(consultation);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return AppointmentViewModel.From(consultation);
    }
}

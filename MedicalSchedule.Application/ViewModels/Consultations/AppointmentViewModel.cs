using System.Linq.Expressions;
using MedicalSchedule.Domain.Entities.Consultations;

namespace MedicalSchedule.Application.ViewModels.Consultations;

public record AppointmentViewModel(
    Guid Id,
    Guid PetId,
    Guid VetId,
    DateTime ScheduledAt,
    string Status,
    string? Notes)
{
    public static Expression<Func<Consultation, AppointmentViewModel>> FromEntity =>
        c => new AppointmentViewModel(c.Id, c.PetId, c.VetId, c.ScheduledAt, c.Status.ToString(), c.Notes);

    public static AppointmentViewModel From(Consultation c) =>
        new(c.Id, c.PetId, c.VetId, c.ScheduledAt, c.Status.ToString(), c.Notes);
}

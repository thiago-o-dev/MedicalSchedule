using System.Linq.Expressions;
using MedicalSchedule.Domain.Entities.Consultations;

namespace MedicalSchedule.Application.ViewModels.Consultations;

public record VetViewModel(Guid Id, string Name, string Crm, string Specialty, bool IsActive)
{
    public static Expression<Func<Vet, VetViewModel>> FromEntity =>
        v => new VetViewModel(v.Id, v.Name, v.Crm, v.Specialty, v.IsActive);

    public static VetViewModel From(Vet v) =>
        new(v.Id, v.Name, v.Crm, v.Specialty, v.IsActive);
}

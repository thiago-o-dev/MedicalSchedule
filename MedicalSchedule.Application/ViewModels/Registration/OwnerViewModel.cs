using System.Linq.Expressions;
using MedicalSchedule.Domain.Entities.Registration;

namespace MedicalSchedule.Application.ViewModels.Registration;

public record OwnerViewModel(
    Guid Id,
    string Name,
    string Cpf,
    string Phone,
    string Email,
    bool IsActive)
{
    public static Expression<Func<Owner, OwnerViewModel>> FromEntity =>
        o => new OwnerViewModel(o.Id, o.Name, o.Cpf, o.Phone, o.Email, o.IsActive);

    public static OwnerViewModel From(Owner o) =>
        new(o.Id, o.Name, o.Cpf, o.Phone, o.Email, o.IsActive);
}

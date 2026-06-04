using System.Linq.Expressions;
using MedicalSchedule.Domain.Entities.Registration;

namespace MedicalSchedule.Application.ViewModels.Registration;

public record PetViewModel(
    Guid Id,
    string Name,
    string Species,
    string Breed,
    DateOnly BirthDate,
    Guid OwnerId,
    bool IsActive)
{
    public static Expression<Func<Pet, PetViewModel>> FromEntity =>
        p => new PetViewModel(p.Id, p.Name, p.Species.ToString(), p.Breed, p.BirthDate, p.OwnerId, p.IsActive);

    public static PetViewModel From(Pet p) =>
        new(p.Id, p.Name, p.Species.ToString(), p.Breed, p.BirthDate, p.OwnerId, p.IsActive);
}

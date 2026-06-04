using MedicalSchedule.Domain.Enums;

namespace MedicalSchedule.Application.ViewModels.Registration;

public record CreatePetViewModel(
    string Name,
    PetSpecies Species,
    string Breed,
    DateOnly BirthDate,
    Guid OwnerId);

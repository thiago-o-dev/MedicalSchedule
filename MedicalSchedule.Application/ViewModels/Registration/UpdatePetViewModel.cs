using MedicalSchedule.Domain.Enums;

namespace MedicalSchedule.Application.ViewModels.Registration;

public record UpdatePetViewModel(string Name, PetSpecies Species, string Breed, DateOnly BirthDate);

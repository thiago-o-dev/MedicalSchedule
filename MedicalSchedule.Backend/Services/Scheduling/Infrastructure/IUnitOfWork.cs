using MedicalSchedule.Domain.Entities.Consultations;
using MedicalSchedule.Domain.Entities.Registration;
using Microsoft.EntityFrameworkCore;

namespace SharedKernel.Abstractions;

public interface IUnitOfWork
{
    DbSet<Owner> Owners { get; }
    DbSet<Pet> Pets { get; }
    DbSet<Vet> Vets { get; }
    DbSet<Consultation> Consultations { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

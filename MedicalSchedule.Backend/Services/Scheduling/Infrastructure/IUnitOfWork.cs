using Microsoft.EntityFrameworkCore;
using Scheduling.Domain.Entities;

namespace Scheduling.Infrastructure;

public interface ISchedulingUnitOfWork
{
    DbSet<Consultation> Consultations { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

using Microsoft.EntityFrameworkCore;
using Scheduling.Domain.Entities;
using Scheduling.Infrastructure;

namespace MedicalSchedule.Backend.UnitTests.Scheduling;

internal sealed class InMemorySchedulingUnitOfWork : DbContext, ISchedulingUnitOfWork
{
    public InMemorySchedulingUnitOfWork(DbContextOptions options) : base(options) { }

    public DbSet<Consultation> Consultations => Set<Consultation>();

    public static InMemorySchedulingUnitOfWork Create()
    {
        var options = new DbContextOptionsBuilder<InMemorySchedulingUnitOfWork>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new InMemorySchedulingUnitOfWork(options);
    }

    public new Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => base.SaveChangesAsync(cancellationToken);
}

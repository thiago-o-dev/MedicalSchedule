using BuildingBlocks.Persistence.EntityFramework;
using BuildingBlocks.Persistence.Models;
using Microsoft.EntityFrameworkCore;
using Scheduling.Domain.Entities;
using Scheduling.Infrastructure;

namespace Scheduling.Infrastructure.Persistence;

public sealed class SchedulingDbContext : AppDbContext, ISchedulingUnitOfWork
{
    public SchedulingDbContext(DbContextOptions<SchedulingDbContext> options)
        : base(options)
    {
    }

    public DbSet<Consultation> Consultations => Set<Consultation>();
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SchedulingDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}

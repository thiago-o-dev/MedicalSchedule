using BuildingBlocks.Persistence.EntityFramework;
using BuildingBlocks.Persistence.Models;
using Microsoft.EntityFrameworkCore;
using Registry.Domain.Entities;

namespace Registry.Infrastructure.Persistence;

public sealed class RegistryDbContext : AppDbContext
{
    public RegistryDbContext(
        DbContextOptions<RegistryDbContext> options)
        : base(options)
    {
    }

    public DbSet<Owner> Owners => Set<Owner>();

    public DbSet<Pet> Pets => Set<Pet>();

    public DbSet<Vet> Vets => Set<Vet>();

    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(RegistryDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
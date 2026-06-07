using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Registry.Infrastructure.Persistence;

public sealed class RegistryDbContextFactory : IDesignTimeDbContextFactory<RegistryDbContext>
{
    public RegistryDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<RegistryDbContext>()
            .UseNpgsql("Host=localhost;Database=registry;Username=postgres;Password=postgres")
            .Options;

        return new RegistryDbContext(options);
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Registry.Domain.Entities;

namespace Registry.Infrastructure.Persistence.Configurations;

public sealed class VetConfiguration : IEntityTypeConfiguration<Vet>
{
    public void Configure(EntityTypeBuilder<Vet> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Crm).HasMaxLength(20).IsRequired();
        builder.Property(x => x.Specialty).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Email).HasMaxLength(200);

        builder.HasIndex(x => x.Crm).IsUnique();
    }
}

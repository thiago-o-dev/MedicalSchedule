using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Registry.Domain.Entities;

namespace Registry.Infrastructure.Persistence.Configurations;

public sealed class OwnerConfiguration : IEntityTypeConfiguration<Owner>
{
    public void Configure(EntityTypeBuilder<Owner> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Email).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Cpf).HasMaxLength(14).IsRequired();
        builder.Property(x => x.Phone).HasMaxLength(20).IsRequired();

        builder.HasIndex(x => x.Cpf).IsUnique();
        builder.HasIndex(x => x.Email).IsUnique();
    }
}

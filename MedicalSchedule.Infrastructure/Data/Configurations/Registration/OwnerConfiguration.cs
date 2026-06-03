using MedicalSchedule.Domain.Entities.Registration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MedicalSchedule.Infrastructure.Data.Configurations.Registration;

public class OwnerConfiguration : IEntityTypeConfiguration<Owner>
{
    public void Configure(EntityTypeBuilder<Owner> builder)
    {
        builder.ToTable("owners");
        builder.HasKey(o => o.Id);

        builder.Property(o => o.Name).IsRequired().HasMaxLength(200);
        builder.Property(o => o.Cpf).IsRequired().HasMaxLength(14);
        builder.Property(o => o.Phone).IsRequired().HasMaxLength(20);
        builder.Property(o => o.Email).IsRequired().HasMaxLength(200);
        builder.Property(o => o.CreatedAt).IsRequired();
        builder.Property(o => o.UpdatedAt).IsRequired();
        builder.Property(o => o.IsActive).IsRequired();
        builder.Property(o => o.RowVersion).IsRequired().IsConcurrencyToken();

        builder.HasIndex(o => o.Cpf).IsUnique();

        builder.HasMany(o => o.Pets)
            .WithOne(p => p.Owner)
            .HasForeignKey(p => p.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

using MedicalSchedule.Domain.Entities.Consultations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MedicalSchedule.Infrastructure.Data.Configurations.Consultations;

public class VetConfiguration : IEntityTypeConfiguration<Vet>
{
    public void Configure(EntityTypeBuilder<Vet> builder)
    {
        builder.ToTable("vets");
        builder.HasKey(v => v.Id);

        builder.Property(v => v.Name).IsRequired().HasMaxLength(200);
        builder.Property(v => v.Crm).IsRequired().HasMaxLength(20);
        builder.Property(v => v.Specialty).IsRequired().HasMaxLength(100);
        builder.Property(v => v.CreatedAt).IsRequired();
        builder.Property(v => v.UpdatedAt).IsRequired();
        builder.Property(v => v.IsActive).IsRequired();
        builder.Property(v => v.RowVersion).IsRequired().IsConcurrencyToken();

        builder.HasIndex(v => v.Crm).IsUnique();
    }
}

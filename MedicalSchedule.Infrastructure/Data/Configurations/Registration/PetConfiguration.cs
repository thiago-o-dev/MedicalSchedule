using MedicalSchedule.Domain.Entities.Registration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MedicalSchedule.Infrastructure.Data.Configurations.Registration;

public class PetConfiguration : IEntityTypeConfiguration<Pet>
{
    public void Configure(EntityTypeBuilder<Pet> builder)
    {
        builder.ToTable("pets");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name).IsRequired().HasMaxLength(100);
        builder.Property(p => p.Species).IsRequired().HasConversion<string>();
        builder.Property(p => p.Breed).IsRequired().HasMaxLength(100);
        builder.Property(p => p.BirthDate).IsRequired();
        builder.Property(p => p.OwnerId).IsRequired();
        builder.Property(p => p.CreatedAt).IsRequired();
        builder.Property(p => p.UpdatedAt).IsRequired();
        builder.Property(p => p.IsActive).IsRequired();
        builder.Property(p => p.RowVersion).IsRequired().IsConcurrencyToken();
    }
}

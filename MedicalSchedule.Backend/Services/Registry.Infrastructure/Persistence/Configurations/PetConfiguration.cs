using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Registry.Domain.Entities;

namespace Registry.Infrastructure.Persistence.Configurations;

public sealed class PetConfiguration : IEntityTypeConfiguration<Pet>
{
    public void Configure(
        EntityTypeBuilder<Pet> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.DeletionStatus)
            .HasConversion<int>()
            .HasDefaultValue(Registry.Domain.Enums.PetDeletionStatus.None)
            .IsRequired();

        builder.Property(x => x.DeletionRejectionReason)
            .HasMaxLength(500);

        builder.HasMany(x => x.Ownerships)
            .WithOne()
            .HasForeignKey(x => x.PetId);
    }
}
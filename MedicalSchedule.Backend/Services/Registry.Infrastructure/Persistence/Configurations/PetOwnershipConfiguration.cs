using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Registry.Domain.Entities;

namespace Registry.Infrastructure.Persistence.Configurations;

public sealed class PetOwnershipConfiguration : IEntityTypeConfiguration<PetOwnership>
{
    public void Configure(EntityTypeBuilder<PetOwnership> builder)
    {
        builder.HasKey(x => new { x.PetId, x.OwnerId });

        builder.HasOne<Owner>()
            .WithMany()
            .HasForeignKey(x => x.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.IsPrimaryOwner).IsRequired();
        builder.Property(x => x.CreatedAt).IsRequired();
    }
}

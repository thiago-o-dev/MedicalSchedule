using MedicalSchedule.Domain.Entities.Consultations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MedicalSchedule.Infrastructure.Data.Configurations.Consultations;

public class ConsultationConfiguration : IEntityTypeConfiguration<Consultation>
{
    public void Configure(EntityTypeBuilder<Consultation> builder)
    {
        builder.ToTable("consultations");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.PetId).IsRequired();
        builder.Property(c => c.VetId).IsRequired();
        builder.Property(c => c.ScheduledAt).IsRequired();
        builder.Property(c => c.Status).IsRequired().HasConversion<string>();
        builder.Property(c => c.Notes).HasMaxLength(1000);
        builder.Property(c => c.CreatedAt).IsRequired();
        builder.Property(c => c.UpdatedAt).IsRequired();
        builder.Property(c => c.IsActive).IsRequired();

        builder.HasOne<MedicalSchedule.Domain.Entities.Registration.Pet>()
            .WithMany()
            .HasForeignKey(c => c.PetId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Vet>()
            .WithMany()
            .HasForeignKey(c => c.VetId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

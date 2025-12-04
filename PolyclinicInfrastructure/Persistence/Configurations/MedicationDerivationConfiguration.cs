using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PolyclinicDomain.Entities;

namespace PolyclinicInfrastructure.Persistence.Configurations;

public class MedicationDerivationConfiguration : IEntityTypeConfiguration<MedicationDerivation>
{
    public void Configure(EntityTypeBuilder<MedicationDerivation> entity)
    {
        entity.ToTable("MedicationDerivation");

        entity.HasKey(md => md.MedicationDerivationId);
        
        entity.HasIndex(md => new {
            md.ConsultationDerivationId,
            md.MedicationId})
            .IsUnique();

        entity.HasOne(md => md.ConsultationDerivation)
            .WithMany(cd => cd.MedicationDerivations)
            .HasForeignKey(md => md.ConsultationDerivationId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        entity.HasOne(md => md.Medication)
            .WithMany(m => m.MedicationDerivations)
            .HasForeignKey(md => md.MedicationId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();
    }
}
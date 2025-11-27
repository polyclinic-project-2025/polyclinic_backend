using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PolyclinicDomain.Entities;

namespace PolyclinicInfrastructure.Persistence.Configurations;

public class MedicationReferralConfiguration : IEntityTypeConfiguration<MedicationReferral>
{
    public void Configure(EntityTypeBuilder<MedicationReferral> entity)
    {
        entity.ToTable("MedicationReferral");

        entity.HasKey(mr => mr.MedicationReferralId);

        entity.HasIndex(mr => new {
            mr.ConsultationReferralId,
            mr.MedicationId})
            .IsUnique();

        entity.HasOne(mr => mr.ConsultationReferral)
            .WithMany(cr => cr.MedicationReferrals)
            .HasForeignKey(mr => mr.ConsultationReferralId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        entity.HasOne(mr => mr.Medication)
            .WithMany(m => m.MedicationReferrals)
            .HasForeignKey(mr => mr.MedicationId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();
    }
}
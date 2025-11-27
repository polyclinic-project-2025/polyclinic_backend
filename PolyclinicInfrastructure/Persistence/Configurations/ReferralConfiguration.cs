using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PolyclinicDomain.Entities;

namespace PolyclinicInfrastructure.Persistence.Configurations;

public class ReferralConfiguration : IEntityTypeConfiguration<Referral>
{
    public void Configure(EntityTypeBuilder<Referral> entity)
    {
        entity.ToTable("Referral");

        entity.HasKey(r => r.ReferralId);

        entity.HasIndex(r => new { 
                r.PatientId, 
                r.DateTimeRem,
                r.ExternalMedicalPostId})
                .IsUnique();

        entity.HasOne(r => r.Patient)
                .WithMany(p => p.Referrals)
                .HasForeignKey(r => r.PatientId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

        entity.HasOne(r => r.ExternalMedicalPost)
                .WithMany()
                .HasForeignKey(r => r.ExternalMedicalPostId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

        entity.HasOne(r => r.DepartmentTo)
                .WithMany(d => d.Referrals)
                .HasForeignKey(r => r.DepartmentToId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();
    }
}
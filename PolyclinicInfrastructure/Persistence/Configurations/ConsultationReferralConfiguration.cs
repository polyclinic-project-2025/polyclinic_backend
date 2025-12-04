using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PolyclinicDomain.Entities;

namespace PolyclinicInfrastructure.Persistence.Configurations;

public class ConsultationReferralConfiguration : IEntityTypeConfiguration<ConsultationReferral>
{
    public void Configure(EntityTypeBuilder<ConsultationReferral> entity)
    {
        entity.ToTable("ConsultationReferral");

            entity.HasKey(cr => cr.ConsultationReferralId);

            entity.HasIndex(c => new {
                c.DoctorId,
                c.ReferralId,
                c.DateTimeCRem})
                .IsUnique();

            entity.HasOne(c => c.DepartmentHead)
                    .WithMany(dh => dh.ConsultationReferrals)
                    .HasForeignKey(c => c.DepartmentHeadId)
                    .HasPrincipalKey(dh => dh.DepartmentHeadId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired();

            entity.HasOne(c => c.Doctor)
                    .WithMany(d => d.ConsultationReferrals)
                    .HasForeignKey(c => c.DoctorId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired();

            entity.HasOne(c => c.Referral)
                    .WithMany(r => r.ConsultationReferrals)
                    .HasForeignKey(c => c.ReferralId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired();
    }
}
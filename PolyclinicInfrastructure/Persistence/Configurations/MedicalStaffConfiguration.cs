using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PolyclinicDomain.Entities;

namespace PolyclinicInfrastructure.Persistence.Configurations;

public class MedicalStaffConfiguration : IEntityTypeConfiguration<MedicalStaff>
{
    public void Configure(EntityTypeBuilder<MedicalStaff> entity)
    {
        entity.ToTable("MedicalStaff");

        entity.HasOne(e => e.Department)
            .WithMany(ms => ms.MedicalStaffs)
            .HasForeignKey(e => e.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();
    }
}
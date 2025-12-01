using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PolyclinicDomain.Entities;

namespace PolyclinicInfrastructure.Persistence.Configurations;

public class DepartmentHeadConfiguration : IEntityTypeConfiguration<DepartmentHead>
{
    public void Configure(EntityTypeBuilder<DepartmentHead> entity)
    {
        entity.ToTable("DepartmentHead");

        entity.HasOne(dh => dh.Doctor)
            .WithOne(d => d.DepartmentHead)
            .HasForeignKey<DepartmentHead>(dh => dh.DoctorId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();
    }
}
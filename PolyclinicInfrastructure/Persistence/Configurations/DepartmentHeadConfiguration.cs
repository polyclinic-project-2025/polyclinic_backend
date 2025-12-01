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

        entity.HasKey(dh => dh.DepartmentHeadId);

        entity.HasOne(dh => dh.Doctor)
            .WithMany(d => d.DepartmentHeads)
            .HasForeignKey(dh => dh.DoctorId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        entity.HasOne(dh => dh.Department)
            .WithMany(d => d.DepartmentHeads)
            .HasForeignKey(dh => dh.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();
    }
}
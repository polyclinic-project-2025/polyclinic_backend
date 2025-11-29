using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PolyclinicDomain.Entities;

namespace PolyclinicInfrastructure.Persistence.Configurations;

public class StockDepartmentConfiguration : IEntityTypeConfiguration<StockDepartment>
{
    public void Configure(EntityTypeBuilder<StockDepartment> entity)
    {
        entity.ToTable("StockDepartment");

        entity.HasKey(sd => sd.StockDepartmentId);
        
        entity.HasIndex(sd => new {
                sd.DepartmentId,
                sd.MedicationId})
                .IsUnique();
            
        entity.HasOne(sd => sd.Medication)
            .WithMany(m => m.StockDepartments)
            .HasForeignKey(sd => sd.MedicationId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();
        
        entity.HasOne(sd => sd.Department)
            .WithMany(d => d.StockDepartments)
            .HasForeignKey(sd => sd.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();
    }
}
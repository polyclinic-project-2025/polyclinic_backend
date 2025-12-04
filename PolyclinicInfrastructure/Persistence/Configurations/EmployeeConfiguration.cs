using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PolyclinicDomain.Entities;
using Microsoft.AspNetCore.Identity;

namespace PolyclinicInfrastructure.Persistence.Configurations;

public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> entity)
    {
        entity.ToTable("Employee");

        entity.HasKey(e => e.EmployeeId);

        entity.HasIndex(e => e.Identification)
            .IsUnique();
        
        entity.HasOne<IdentityUser>()
        .WithOne()
        .HasForeignKey<Employee>(e => e.UserId)
        .OnDelete(DeleteBehavior.SetNull);
        
        entity.HasIndex(e => e.UserId)
                .IsUnique();
    }
}
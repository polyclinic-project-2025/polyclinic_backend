using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PolyclinicDomain.Entities;

namespace PolyclinicInfrastructure.Persistence.Configurations;

public class NurseConfiguration : IEntityTypeConfiguration<Nurse>
{
    public void Configure(EntityTypeBuilder<Nurse> entity)
    {
        entity.ToTable("Nurse");

        entity.HasOne(n => n.Nursing)
            .WithMany(nu => nu.Nurses)
            .HasForeignKey(n => n.NursingId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();
    }
}
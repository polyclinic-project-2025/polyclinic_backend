using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PolyclinicDomain.Entities;

namespace PolyclinicInfrastructure.Persistence.Configurations;

public class NursingConfiguration : IEntityTypeConfiguration<Nursing>
{
    public void Configure(EntityTypeBuilder<Nursing> entity)
    {
        entity.ToTable("Nursing");

        entity.HasKey(e => e.NursingId);
    }
}
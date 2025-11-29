using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PolyclinicDomain.Entities;

namespace PolyclinicInfrastructure.Persistence.Configurations;

public class ExternalMedicalPostConfiguration : IEntityTypeConfiguration<ExternalMedicalPost>
{
    public void Configure(EntityTypeBuilder<ExternalMedicalPost> entity)
    {
        entity.ToTable("ExternalMedicalPost");

        entity.HasKey(e => e.ExternalMedicalPostId);
    }
}
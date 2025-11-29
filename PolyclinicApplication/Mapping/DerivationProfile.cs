using AutoMapper;
using System;
using PolyclinicDomain.Entities;
using PolyclinicApplication.DTOs.Request.Derivations;
using PolyclinicApplication.DTOs.Response.Derivations;


namespace PolyclinicApplication.Mapping{
public class DerivationProfile : Profile
{
    public DerivationProfile()
    {
        // Entity → DTO
        CreateMap<Derivation, DerivationDto>()
            .ForMember(dest => dest.DepartmentFromName,
                opt => opt.MapFrom(src => src.DepartmentFrom != null ? src.DepartmentFrom.Name : null))
            .ForMember(dest => dest.DepartmentToName,
                opt => opt.MapFrom(src => src.DepartmentTo != null ? src.DepartmentTo.Name : null))
            .ForMember(dest => dest.PatientName,
                opt => opt.MapFrom(src => src.Patient != null ? src.Patient.Name : null));

        // CreateDto → Entity
        CreateMap<CreateDerivationDto, Derivation>();
    }
}
}


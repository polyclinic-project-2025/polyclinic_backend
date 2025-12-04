using AutoMapper;
using System;
using PolyclinicDomain.Entities;
using PolyclinicApplication.DTOs.Request.Referral;
using PolyclinicApplication.DTOs.Response.Referral;


namespace PolyclinicApplication.Mapping{
public class ReferralProfile : Profile
{
    public ReferralProfile()
    {
        // Entity → DTO
        CreateMap<Referral, ReferralDto>()
            .ForMember(dest => dest.PuestoExterno,
                opt => opt.MapFrom(src => src.ExternalMedicalPost != null ? src.ExternalMedicalPost.Name : null))
            .ForMember(dest => dest.DepartmentToName,
                opt => opt.MapFrom(src => src.DepartmentTo != null ? src.DepartmentTo.Name : null))
            .ForMember(dest => dest.PatientName,
                opt => opt.MapFrom(src => src.Patient != null ? src.Patient.Name : null));

        // CreateDto → Entity
        CreateMap<CreateReferralDto, Referral>();
    }
}
}

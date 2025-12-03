using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using PolyclinicDomain.Entities;
using PolyclinicApplication.DTOs.Request;
using PolyclinicApplication.DTOs.Response;
using PolyclinicApplication.Mapping;

namespace PolyclinicApplication.Mapping;

public class ConsultationDerivationProfile : Profile
{
    public ConsultationDerivationProfile()
    {
        CreateMap<CreateConsultationDerivationDto, ConsultationDerivation>();
        CreateMap<UpdateConsultationDerivationDto, ConsultationDerivation>();
        CreateMap<ConsultationDerivation, ConsultationDerivationDto>()
            .ForMember(dest => dest.PatientId,
                opt => opt.MapFrom(src => src.Derivation!.PatientId))
            
            .ForMember(dest => dest.PatientName,
                opt => opt.MapFrom(src => src.Derivation!.Patient!.Name))
                
            .ForMember(dest => dest.DepartmentToId,
                opt => opt.MapFrom(src => src.Derivation!.DepartmentToId))
            
            .ForMember(dest => dest.DepartmentToName,
                opt => opt.MapFrom(src => src.Derivation!.DepartmentTo!.Name))

            .ForMember(dest => dest.DoctorName,
                opt => opt.MapFrom(src => src.Doctor!.Name));

    }
}
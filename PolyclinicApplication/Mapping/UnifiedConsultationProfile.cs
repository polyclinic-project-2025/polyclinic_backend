// UnifiedConsultationProfile.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using PolyclinicDomain.Entities;
using PolyclinicApplication.DTOs.Response;

namespace PolyclinicApplication.Mappings;

public class UnifiedConsultationProfile : Profile
{
    public UnifiedConsultationProfile()
    {
        // Mapeo desde ConsultationDerivation
        CreateMap<ConsultationDerivation, UnifiedConsultationDto>()
            .ForMember(dest => dest.Id, 
                opt => opt.MapFrom(src => src.ConsultationDerivationId))
            .ForMember(dest => dest.Type, 
                opt => opt.MapFrom(src => "Derivation"))
            .ForMember(dest => dest.ConsultationDate, 
                opt => opt.MapFrom(src => src.DateTimeCDer))
            .ForMember(dest => dest.Diagnosis, 
                opt => opt.MapFrom(src => src.Diagnosis ?? string.Empty))
            .ForMember(dest => dest.PatientId, 
                opt => opt.MapFrom(src => src.Derivation != null ? src.Derivation.PatientId : Guid.Empty))
            .ForMember(dest => dest.PatientFullName, 
                opt => opt.MapFrom(src => src.Derivation != null && src.Derivation.Patient != null 
                    ? src.Derivation.Patient.Name 
                    : "Desconocido"))
            .ForMember(dest => dest.DoctorId, 
                opt => opt.MapFrom(src => src.DoctorId))
            .ForMember(dest => dest.DoctorFullName, 
                opt => opt.MapFrom(src => src.Doctor != null 
                    ? src.Doctor.Name 
                    : "Desconocido"))
            .ForMember(dest => dest.DepartmentId, 
                opt => opt.MapFrom(src => src.Derivation != null 
                    ? src.Derivation.DepartmentToId 
                    : Guid.Empty))
            .ForMember(dest => dest.DepartmentName, 
                opt => opt.MapFrom(src => src.Derivation != null && src.Derivation.DepartmentTo != null 
                    ? src.Derivation.DepartmentTo.Name 
                    : "Desconocido"));

        // Mapeo desde ConsultationReferral
        CreateMap<ConsultationReferral, UnifiedConsultationDto>()
            .ForMember(dest => dest.Id, 
                opt => opt.MapFrom(src => src.ConsultationReferralId))
            .ForMember(dest => dest.Type, 
                opt => opt.MapFrom(src => "Referral"))
            .ForMember(dest => dest.ConsultationDate, 
                opt => opt.MapFrom(src => src.DateTimeCRem))
            .ForMember(dest => dest.Diagnosis, 
                opt => opt.MapFrom(src => src.Diagnosis ?? string.Empty))
            .ForMember(dest => dest.PatientId, 
                opt => opt.MapFrom(src => src.Referral != null ? src.Referral.PatientId : Guid.Empty))
            .ForMember(dest => dest.PatientFullName, 
                opt => opt.MapFrom(src => src.Referral != null && src.Referral.Patient != null 
                    ? src.Referral.Patient.Name 
                    : "Desconocido"))
            .ForMember(dest => dest.DoctorId, 
                opt => opt.MapFrom(src => src.DoctorId))
            .ForMember(dest => dest.DoctorFullName, 
                opt => opt.MapFrom(src => src.Doctor != null 
                    ? src.Doctor.Name 
                    : "Desconocido"))
            .ForMember(dest => dest.DepartmentId, 
                opt => opt.MapFrom(src => src.Referral != null 
                    ? src.Referral.DepartmentToId 
                    : Guid.Empty))
            .ForMember(dest => dest.DepartmentName, 
                opt => opt.MapFrom(src => src.Referral != null && src.Referral.DepartmentTo != null 
                    ? src.Referral.DepartmentTo.Name 
                    : "Desconocido"));
    }
}
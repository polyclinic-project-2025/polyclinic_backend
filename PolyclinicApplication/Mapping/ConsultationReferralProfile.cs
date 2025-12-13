using AutoMapper;
using PolyclinicApplication.DTOs.Request.Consultations;
using PolyclinicApplication.DTOs.Response.Consultations;
using PolyclinicDomain.Entities;

namespace PolyclinicApplication.Mapping;

public class ConsultationReferralProfile : Profile
{
    public ConsultationReferralProfile()
    {
        // Entity -> Response
        CreateMap<ConsultationReferral, ConsultationReferralResponse>()
            .ForMember(dest => dest.PatientId,
                opt => opt.MapFrom(src => src.Referral!.PatientId))
            
            .ForMember(dest => dest.PatientFullName,
                opt => opt.MapFrom(src => src.Referral!.Patient!.Name))
                
            .ForMember(dest => dest.DepartmentToId,
                opt => opt.MapFrom(src => src.Referral!.DepartmentToId))
            
            .ForMember(dest => dest.DepartmentName,
                opt => opt.MapFrom(src => src.Referral!.DepartmentTo!.Name))

            .ForMember(dest => dest.DoctorFullName,
                opt => opt.MapFrom(src => src.Doctor!.Name));

        // CreateDto -> Entity
        CreateMap<CreateConsultationReferralDto, ConsultationReferral>()
            .ConstructUsing(dto => new ConsultationReferral(
                Guid.NewGuid(),
                dto.Diagnosis,
                dto.DepartmentHeadId,
                dto.DoctorId,
                dto.ReferralId,
                dto.DateTimeCRem));

        // UpdateDto -> Entity (for mapping non-null values)
        CreateMap<UpdateConsultationReferralDto, ConsultationReferral>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
    }
}
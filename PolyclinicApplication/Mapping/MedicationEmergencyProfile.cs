using AutoMapper;
using PolyclinicDomain.Entities;
using PolyclinicApplication.DTOs.Request;
using PolyclinicApplication.DTOs.Response;

namespace PolyclinicApplication.Mapping
{
    public class MedicationEmergencyProfile : Profile
    {
        public MedicationEmergencyProfile()
        {
            // Request DTOs -> Entity
            CreateMap<CreateMedicationEmergencyDto, MedicationEmergency>();
            
            CreateMap<UpdateMedicationEmergencyDto, MedicationEmergency>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => 
                    srcMember != null)); // Solo mapea si el valor no es null
            
            // Entity -> Response DTO
            CreateMap<MedicationEmergency, MedicationEmergencyDto>()
                .ForMember(dest => dest.CommercialName,
                    opt => opt.MapFrom(src => 
                        src.Medication != null ? src.Medication.CommercialName : string.Empty));
        }
    }
}
using AutoMapper;
using PolyclinicDomain.Entities;
using PolyclinicApplication.DTOs.Request;
using PolyclinicApplication.DTOs.Response;

namespace PolyclinicApplication.Mapping
{
    public class EmergencyRoomProfile : Profile
    {
        public EmergencyRoomProfile()
        {
            // Request DTOs -> Entity
            CreateMap<CreateEmergencyRoomDto, EmergencyRoom>();
            
            CreateMap<UpdateEmergencyRoomDto, EmergencyRoom>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => 
                    srcMember != null)); // Solo mapea si el valor no es null
            
            // Entity -> Response DTO
            CreateMap<EmergencyRoom, EmergencyRoomDto>()
                .ForMember(dest => dest.DoctorName,
                    opt => opt.MapFrom(src => src.Doctor != null ? src.Doctor.Name : string.Empty))
                
                .ForMember(dest => dest.DoctorIdentification,
                    opt => opt.MapFrom(src => src.Doctor != null ? src.Doctor.Identification : string.Empty));
        }
    }
}
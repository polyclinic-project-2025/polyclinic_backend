using AutoMapper;
using PolyclinicDomain.Entities;
using PolyclinicApplication.DTOs.Request;
using PolyclinicApplication.DTOs.Response;

namespace PolyclinicApplication.Mapping
{
    public class EmergencyRoomCareProfile : Profile
    {
        public EmergencyRoomCareProfile()
        {
            // Request DTOs -> Entity
            CreateMap<CreateEmergencyRoomCareDto, EmergencyRoomCare>();
            
            CreateMap<UpdateEmergencyRoomCareDto, EmergencyRoomCare>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => 
                    srcMember != null)); // Solo mapea si el valor no es null
            
            // Entity -> Response DTO
            CreateMap<EmergencyRoomCare, EmergencyRoomCareDto>()
                // Datos del paciente
                .ForMember(dest => dest.PatientName,
                    opt => opt.MapFrom(src => src.Patient != null ? src.Patient.Name : string.Empty))
                
                .ForMember(dest => dest.PatientIdentification,
                    opt => opt.MapFrom(src => src.Patient != null ? src.Patient.Identification : string.Empty))
                
                // Datos del doctor (a través de EmergencyRoom)
                .ForMember(dest => dest.DoctorId,
                    opt => opt.MapFrom(src => 
                        src.EmergencyRoom != null && src.EmergencyRoom.Doctor != null 
                        ? src.EmergencyRoom.Doctor.EmployeeId 
                        : Guid.Empty))
                
                .ForMember(dest => dest.DoctorName,
                    opt => opt.MapFrom(src => 
                        src.EmergencyRoom != null && src.EmergencyRoom.Doctor != null 
                        ? src.EmergencyRoom.Doctor.Name 
                        : string.Empty))
                
                .ForMember(dest => dest.DoctorIdentification,
                    opt => opt.MapFrom(src => 
                        src.EmergencyRoom != null && src.EmergencyRoom.Doctor != null 
                        ? src.EmergencyRoom.Doctor.Identification 
                        : string.Empty));
        }
    }
}
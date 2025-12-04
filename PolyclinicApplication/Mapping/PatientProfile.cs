using AutoMapper;
using PolyclinicDomain.Entities;
using PolyclinicApplication.DTOs.Request.Patients;
using PolyclinicApplication.DTOs.Response.Patients;

namespace PolyclinicApplication.Mapping
{
    public class PatientProfile : Profile
    {
        public PatientProfile()
        {
            // Mapear de entidad a DTO de respuesta
            CreateMap<Patient, PatientDto>().ReverseMap();

            // Mapear de DTO de creación a entidad
            CreateMap<CreatePatientDto, Patient>();

            // Mapear de DTO de actualización a entidad
            CreateMap<UpdatePatientDto, Patient>();
        }
    }
}

using AutoMapper;
using PolyclinicDomain.Entities;
using PolyclinicApplication.DTOs.Request;
using PolyclinicApplication.DTOs.Response;

namespace PolyclinicApplication.Mapping
{
    public class PuestoExternoProfile : Profile
    {
        public PuestoExternoProfile()
        {
            CreateMap<ExternalMedicalPost, PuestoExternoDto>();
            CreateMap<CreatePuestoExternoDto, ExternalMedicalPost>();
        }
    }
}

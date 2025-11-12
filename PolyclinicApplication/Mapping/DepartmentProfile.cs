using AutoMapper;
using PolyclinicDomain.Entities;
using PolyclinicApplication.DTOs.Departments;

namespace PolyclinicApplication.Mappings
{
    public class DepartmentProfile : Profile
    {
        public DepartmentProfile()
        {
            CreateMap<Department, CreateDepartmentDto>().ReverseMap();
            CreateMap<Department, UpdateDepartmentDto>().ReverseMap();
        }
    }
}

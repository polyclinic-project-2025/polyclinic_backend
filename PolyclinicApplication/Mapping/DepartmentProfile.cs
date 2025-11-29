using AutoMapper;
using PolyclinicDomain.Entities;
using PolyclinicApplication.DTOs.Departments;

namespace PolyclinicApplication.Mapping
{
    public class DepartmentProfile : Profile
    {
        public DepartmentProfile()
        {
            CreateMap<Department, DepartmentDto>().ReverseMap();
            CreateMap<CreateDepartmentDto, Department>();
            CreateMap<UpdateDepartmentDto, Department>();
        }
    }
}

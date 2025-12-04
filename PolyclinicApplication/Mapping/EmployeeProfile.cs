using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PolyclinicApplication.DTOs.Response;
using PolyclinicDomain.Entities;
using AutoMapper;

namespace PolyclinicApplication.Mapping;

public class EmployeeProfile : Profile
{
    public EmployeeProfile()
    {
        CreateMap<Employee, EmployeeResponse>().ReverseMap();
    }   
}
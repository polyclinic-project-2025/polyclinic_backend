using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using PolyclinicDomain.Entities;
using PolyclinicApplication.DTOs.Request;
using PolyclinicApplication.DTOs.Response;

namespace PolyclinicApplication.Mapping;

public class NurseProfile : Profile
{
    public NurseProfile()
    {
        CreateMap<Nurse, NurseResponse>().ReverseMap();
        CreateMap<CreateNurseRequest, Nurse>();
        CreateMap<UpdateNurseRequest, Nurse>();
    }
}
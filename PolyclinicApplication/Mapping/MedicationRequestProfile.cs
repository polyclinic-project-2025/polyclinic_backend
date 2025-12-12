using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using PolyclinicDomain.Entities;
using PolyclinicApplication.DTOs.Request;
using PolyclinicApplication.DTOs.Response;

namespace PolyclinicApplication.Mapping;

public class MedicationRequestProfile : Profile
{
    public MedicationRequestProfile()
    {
        CreateMap<MedicationRequest, MedicationRequestResponse>().ReverseMap();
        CreateMap<CreateMedicationRequestRequest, MedicationRequest>();
        CreateMap<UpdateMedicationRequestRequest, MedicationRequest>();
    }   
}
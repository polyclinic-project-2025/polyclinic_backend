using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using PolyclinicDomain.Entities;
using PolyclinicApplication.DTOs.Request;
using PolyclinicApplication.DTOs.Response;

namespace PolyclinicApplication.Mapping;

public class WarehouseRequestProfile : Profile
{
    public WarehouseRequestProfile()
    {
        CreateMap<WarehouseRequest, WarehouseRequestResponse>().ReverseMap();
        CreateMap<CreateWarehouseRequestRequest, WarehouseRequest>();
        CreateMap<UpdateWarehouseRequestRequest, WarehouseRequest>();
    }
}
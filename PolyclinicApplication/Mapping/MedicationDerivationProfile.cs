using AutoMapper;
using PolyclinicDomain.Entities;
using PolyclinicApplication.DTOs.Request.MedicationDerivation;
using PolyclinicApplication.DTOs.Response;

namespace PolyclinicApplication.Mapping;

public class MedicationDerivationProfile : Profile
{
    public MedicationDerivationProfile()
    {
        CreateMap<MedicationDerivation, MedicationDerivationDto>();
        CreateMap<CreateMedicationDerivationDto, MedicationDerivation>();
        CreateMap<UpdateMedicationDerivationDto, MedicationDerivation>();
    }
}
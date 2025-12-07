using AutoMapper;
using PolyclinicDomain.Entities;
using PolyclinicApplication.DTOs.Request.MedicationReferrals;
using PolyclinicApplication.DTOs.Response.MedicationReferrals;

namespace PolyclinicApplication.Mapping;

public class MedicationReferralProfile : Profile
{
    public MedicationReferralProfile()
    {
        CreateMap<MedicationReferral, MedicationReferralDto>();
        CreateMap<CreateMedicationReferralDto, MedicationReferral>();
        CreateMap<UpdateMedicationReferralDto, MedicationReferral>();
    }
}

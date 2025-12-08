using AutoMapper;
using PolyclinicDomain.Entities;
using PolyclinicApplication.DTOs.Request.StockDepartment;
using PolyclinicApplication.DTOs.Response;

namespace PolyclinicApplication.Mapping
{
    public class StockDepartmentProfile : Profile
    {
        public StockDepartmentProfile()
        {
            // Entity → DTO
            CreateMap<StockDepartment, StockDepartmentDto>()
                .ForMember(dest => dest.MedicationCommercialName,
                           opt => opt.MapFrom(src => src.Medication != null ? src.Medication.CommercialName : string.Empty))
                .ForMember(dest => dest.MedicationScientificName,
                           opt => opt.MapFrom(src => src.Medication != null ? src.Medication.ScientificName : string.Empty));

            // CreateDto → Entity
            CreateMap<CreateStockDepartmentDto, StockDepartment>();

            // UpdateDto → Entity
            CreateMap<UpdateStockDepartmentDto, StockDepartment>();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PolyclinicApplication.DTOs.Request;
using PolyclinicApplication.DTOs.Response;

namespace PolyclinicApplication.Services.Interfaces
{
    public interface IPuestoExternoService
    {
        Task<PuestoExternoDto> CreateAsync(CreatePuestoExternoDto dto);
        Task<IEnumerable<PuestoExternoDto>> GetAllAsync();
        Task<PuestoExternoDto?> GetByIdAsync(Guid id);
        Task DeleteAsync(Guid id);
    }
}
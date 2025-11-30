using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PolyclinicApplication.DTOs.Request;
using PolyclinicApplication.DTOs.Response;
using PolyclinicApplication.Common.Results;

namespace PolyclinicApplication.Services.Interfaces;

public interface IDoctorService
{
    Task<Result<IEnumerable<DoctorResponse>>> GetAllAsync();
    Task<Result<DoctorResponse>> GetByIdAsync(Guid id);
    Task<Result<DoctorResponse>> CreateAsync(CreateDoctorRequest request);
    Task<Result<bool>> UpdateAsync(Guid id, UpdateDoctorRequest request);
    Task<Result<bool>> DeleteAsync(Guid id);
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PolyclinicApplication.Common.Results;
using PolyclinicApplication.DTOs.Response;
using PolyclinicApplication.DTOs.Request;

namespace PolyclinicApplication.Services.Interfaces;

public interface IDepartmentHeadService
{
    Task<Result<IEnumerable<DepartmentHeadResponse>>> GetAllDepartmentHeadAsync();
    Task<Result<DepartmentHeadResponse>> GetDepartmentHeadByIdAsync(Guid id);
    Task<Result<DepartmentHeadResponse>> GetDepartmentHeadByDepartmentIdAsync(Guid departmentId);
    Task<Result<DepartmentHeadResponse>> AssignDepartmentHeadAsync(AssignDepartmentHeadRequest request);
    Task<Result<bool>> RemoveDepartmentHeadAsync(Guid doctorId);
}
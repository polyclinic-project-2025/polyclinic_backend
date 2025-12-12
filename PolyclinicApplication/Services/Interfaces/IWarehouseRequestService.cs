using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PolyclinicApplication.Common.Results;
using PolyclinicApplication.DTOs.Response;
using PolyclinicApplication.DTOs.Request;

namespace PolyclinicApplication.Services.Interfaces;

public interface IWarehouseRequestService
{
    Task<Result<IEnumerable<WarehouseRequestResponse>>> GetAllWarehouseRequestAsync();
    Task<Result<WarehouseRequestResponse>> GetWarehouseRequestByIdAsync(Guid id);
    Task<Result<IEnumerable<WarehouseRequestResponse>>> GetWarehouseRequestByStatusAsync(string status);
    Task<Result<IEnumerable<WarehouseRequestResponse>>> GetWarehouseRequestByDepartmentIdAsync(Guid DepartmentId);
    Task<Result<IEnumerable<WarehouseRequestResponse>>> GetWarehouseRequestByStatusAndDepartmentIdAsync(string status, Guid DepartmentId);
    Task<Result<WarehouseRequestResponse>> CreateWarehouseRequestAsync(CreateWarehouseRequestRequest request);
    Task<Result<bool>> UpdateWarehouseRequestAsync(Guid id, UpdateWarehouseRequestRequest request);
    Task<Result<bool>> DeleteWarehouseRequestAsync(Guid id);
}
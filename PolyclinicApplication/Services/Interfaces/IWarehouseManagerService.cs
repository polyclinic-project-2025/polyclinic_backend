using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PolyclinicApplication.Common.Results;
using PolyclinicApplication.DTOs.Response;
using PolyclinicApplication.DTOs.Request;

namespace PolyclinicApplication.Services.Interfaces;

public interface IWarehouseManagerService :
    IEmployeeService<WarehouseManagerResponse>
{
    Task<Result<WarehouseManagerResponse>> GetWarehouseManagerAsync();
    Task<Result<WarehouseManagerResponse>> CreateAsync(CreateWarehouseManagerRequest request);
    Task<Result<bool>> UpdateAsync(Guid id, UpdateWarehouseManagerRequest request);
}
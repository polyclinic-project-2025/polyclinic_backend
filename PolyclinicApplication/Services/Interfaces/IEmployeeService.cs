using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PolyclinicApplication.DTOs.Request;
using PolyclinicApplication.DTOs.Response;
using PolyclinicApplication.Common.Results;

namespace PolyclinicApplication.Services.Interfaces;

public interface IEmployeeService<TResponse>
{
    Task<Result<IEnumerable<TResponse>>> GetAllAsync();
    Task<Result<TResponse>> GetByIdAsync(Guid id);
    Task<Result<bool>> DeleteAsync(Guid id);
}
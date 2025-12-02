using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PolyclinicApplication.DTOs.Request;
using PolyclinicApplication.DTOs.Response;
using PolyclinicApplication.Common.Results;

namespace PolyclinicApplication.Services.Interfaces;

public interface INurseService :
    IEmployeeService<NurseResponse>
{
    Task<Result<NurseResponse>> CreateAsync(CreateNurseRequest request);
    Task<Result<bool>> UpdateAsync(Guid id, UpdateNurseRequest request);
}
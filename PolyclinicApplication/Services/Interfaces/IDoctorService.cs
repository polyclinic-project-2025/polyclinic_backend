using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PolyclinicApplication.DTOs.Request;
using PolyclinicApplication.DTOs.Response;
using PolyclinicApplication.Common.Results;

namespace PolyclinicApplication.Services.Interfaces;

public interface IDoctorService : 
    IEmployeeService<DoctorResponse>
{
    Task<Result<DoctorResponse>> CreateAsync(CreateDoctorRequest request);
    Task<Result<bool>> UpdateAsync(Guid id, UpdateDoctorRequest request);
}
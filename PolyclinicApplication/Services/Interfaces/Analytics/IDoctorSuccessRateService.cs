using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PolyclinicApplication.Common.Results;
using PolyclinicApplication.ReadModels;

namespace PolyclinicApplication.Services.Interfaces.Analytics;

public interface IDoctorSuccessRateService
{
    Task<Result<IEnumerable<DoctorSuccessRateReadModel>>> GetTop5DoctorsSuccessRateAsync(int frequency);
}
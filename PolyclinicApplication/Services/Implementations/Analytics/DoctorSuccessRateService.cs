using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PolyclinicApplication.QueryInterfaces;
using PolyclinicApplication.ReadModels;
using PolyclinicApplication.Services.Interfaces.Analytics;

namespace PolyclinicApplication.Services.Implementations.Analytics;

public class DoctorSuccessRateService : IDoctorSuccessRateService
{
    private readonly IDoctorSuccessRateQuery _query;

    public DoctorSuccessRateService(IDoctorSuccessRateQuery query)
    {
        _query = query;
    }

    public async Task<IEnumerable<DoctorSuccessRateReadModel>> GetTop5DoctorsSuccessRateAsync(int frequency)
        => await _query.GetTop5DoctorsAsync(frequency);
}
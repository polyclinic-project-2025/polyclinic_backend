using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PolyclinicApplication.QueryInterfaces;
using PolyclinicApplication.ReadModels;
using PolyclinicApplication.Services.Interfaces.Analytics;

namespace PolyclinicApplication.Services.Implementations.Analytics;

public class DoctorMonthlyAverageService : IDoctorMonthlyAverageService
{
    private readonly IDoctorMonthlyAverageQuery _query;

    public DoctorMonthlyAverageService(IDoctorMonthlyAverageQuery query)
    {
        _query = query;
    }
    public async Task<IEnumerable<DoctorMonthlyAverageReadModel>> GetDoctorAverageAsync(DateTime from, DateTime to)
        => await _query.GetDoctorAverageAsync(from, to);
}
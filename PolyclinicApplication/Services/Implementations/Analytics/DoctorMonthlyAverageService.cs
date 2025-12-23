using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PolyclinicApplication.Common.Results;
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
    public async Task<Result<IEnumerable<DoctorMonthlyAverageReadModel>>> GetDoctorAverageAsync(DateTime from, DateTime to)
    {
        if(to < from)
            (from, to) = (to, from);
        
        try
        {
            var result = await _query.GetDoctorAverageAsync(from, to);
            return Result<IEnumerable<DoctorMonthlyAverageReadModel>>.Success(result);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<DoctorMonthlyAverageReadModel>>
                    .Failure($"Error al obtener solicitud: {ex.Message}");
        }
    }
}
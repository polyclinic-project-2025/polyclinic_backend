using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PolyclinicApplication.Common.Results;
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

    public async Task<Result<IEnumerable<DoctorSuccessRateReadModel>>> GetTop5DoctorsSuccessRateAsync(int frequency)
    {
        if(frequency < 1 || frequency > 1000000)
            return Result<IEnumerable<DoctorSuccessRateReadModel>>
                    .Failure("La frecuencia de un medicamento debe ser un entero entre 1 y 1000000.");
                    
        try
        {
            var result = await _query.GetTop5DoctorsAsync(frequency);
            return Result<IEnumerable<DoctorSuccessRateReadModel>>.Success(result);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<DoctorSuccessRateReadModel>>
                    .Failure($"Error al obtener solicitud: {ex.Message}");
        }
    }
}
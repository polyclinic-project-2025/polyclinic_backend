using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PolyclinicApplication.Common.Results;
using PolyclinicApplication.QueryInterfaces;
using PolyclinicApplication.ReadModels;
using PolyclinicApplication.Services.Interfaces.Analytics;

namespace PolyclinicApplication.Services.Implementations.Analytics;

public class PatientListService : IPatientListService
{
    private readonly IPatientListQuery _query;

    public PatientListService(IPatientListQuery query)
    {
        _query = query;
    }

    public async Task<Result<IEnumerable<PatientListReadModel>>> GetPatientsListAsync()
    {
        try
        {
            var result = await _query.GetPatientsListAsync();
            return Result<IEnumerable<PatientListReadModel>>.Success(result);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<PatientListReadModel>>
                    .Failure($"Error al obtener la lista de pacientes");
        }
    }
}
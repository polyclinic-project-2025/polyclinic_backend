using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PolyclinicApplication.Common.Results;
using PolyclinicApplication.QueryInterfaces;
using PolyclinicApplication.ReadModels;
using PolyclinicApplication.Services.Interfaces.Analytics;

namespace PolyclinicApplication.Services.Implementations.Analytics;

public class DeniedWarehouseRequestsService : IDeniedWarehouseRequestsService
{
    private readonly IDeniedWarehouseRequestsQuery _query;

    public DeniedWarehouseRequestsService(IDeniedWarehouseRequestsQuery query)
    {
        _query = query;
    }

    public async Task<Result<IEnumerable<DeniedWarehouseRequestReadModel>>> GetDeniedWarehouseRequestsAsync(string status)
    {
        try
        {
            var result = await _query.GetDeniedAsync(status);
            return Result<IEnumerable<DeniedWarehouseRequestReadModel>>.Success(result);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<DeniedWarehouseRequestReadModel>>
                    .Failure($"Error al obtener solicitud: {ex.Message}");
        }
    } 
}
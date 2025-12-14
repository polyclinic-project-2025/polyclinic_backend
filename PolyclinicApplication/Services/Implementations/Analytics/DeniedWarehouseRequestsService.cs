using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

    public async Task<IEnumerable<DeniedWarehouseRequestReadModel>> GetDeniedWarehouseRequestsAsync(string status)
        => await _query.GetDeniedAsync(status);
}
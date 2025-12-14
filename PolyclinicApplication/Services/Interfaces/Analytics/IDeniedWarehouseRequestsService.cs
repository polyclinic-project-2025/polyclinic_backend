using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PolyclinicApplication.ReadModels;

namespace PolyclinicApplication.Services.Interfaces.Analytics;

public interface IDeniedWarehouseRequestsService
{
    Task<IEnumerable<DeniedWarehouseRequestReadModel>> GetDeniedWarehouseRequestsAsync(string status);
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PolyclinicApplication.ReadModels;

namespace PolyclinicApplication.QueryInterfaces;

public interface IDeniedWarehouseRequestsQuery
{
    Task<IEnumerable<DeniedWarehouseRequestReadModel>> GetDeniedAsync(string status);
}

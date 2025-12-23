using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PolyclinicDomain.Entities;

namespace PolyclinicDomain.IRepositories;

public interface IWarehouseRequestRepository : 
    IRepository<WarehouseRequest>
{
    Task<IEnumerable<WarehouseRequest>> GetByStatusAsync(string status);
    Task<IEnumerable<WarehouseRequest>> GetByDepartmentIdAsync(Guid DepartmentId);
    Task<IEnumerable<WarehouseRequest>> GetByStatusAndDepartmentIdAsync(string status, Guid DepartmentId);
}
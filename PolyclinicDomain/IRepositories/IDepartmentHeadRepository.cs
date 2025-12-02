using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PolyclinicDomain.Entities;

namespace PolyclinicDomain.IRepositories;

public interface IDepartmentHeadRepository : IRepository<DepartmentHead>
{
    Task<DepartmentHead?> GetByDepartmentIdAsync(Guid departmentId);
    Task<DepartmentHead?> GetByIdentificationAsync(string identification);
}
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks; 
using PolyclinicDomain.Entities;

namespace PolyclinicDomain.IRepositories;

public interface IDepartmentHeadRepository : IRepository<DepartmentHead>
{
    Task<DepartmentHead?> GetByDepartmentAsync(Guid managedDepartmentId);
}
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;   
using PolyclinicDomain.Entities;

namespace PolyclinicDomain.IRepositories;

public interface IEmployeeRepository : IRepository<Employee>
{
    Task<Employee?> GetByIdentificationAsync(string identification);
    Task<IEnumerable<Employee>> GetByNameAsync(string name);
    Task<IEnumerable<Employee>> GetByEmploymentStatusAsync(string status);
    Task<IEnumerable<Employee>> GetByPrimaryRoleAsync(string role);
}
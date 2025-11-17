using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks; 
using PolyclinicDomain.Entities;

namespace PolyclinicDomain.IRepositories;

public interface IEmployeeRepository : IRepository<Employee>
{
    Task<Employee> GetByIdentificationAsync(string identification);
    Task<Employee> GetByNameAsync(string name);
    Task<Employee> GetByEmploymentStatusAsync(string employmentStatus);
    Task<Employee> GetByPrimaryRoleAsync(string primaryRole);
}
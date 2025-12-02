using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PolyclinicDomain.Entities;

namespace PolyclinicDomain.IRepositories;

public interface IEmployeeRepository <TEntity> : 
    IRepository<TEntity>
    where TEntity : Employee
{
    Task<TEntity?> GetByIdentificationAsync(string identification);
    Task<bool> ExistsByIdentificationAsync(string identification);
}
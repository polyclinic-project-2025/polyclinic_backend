using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using PolyclinicDomain.Entities;

namespace PolyclinicDomain.IRepositories;

public interface IDepartmentRepository : IRepository<Department>
{
    // Devuelve el departamento con sus relaciones principales cargadas (Head, MedicalStaff, Stock)
    Task<Department?> GetWithHeadAsync(Guid id);

    // Opcionales, útiles para reglas de negocio
    Task<Department?> GetByNameAsync(string name);
    Task<bool> ExistsByNameAsync(string name);
}

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using PolyclinicDomain.Entities;

namespace PolyclinicDomain.IRepositories;

public interface IPuestoExternoRepository : IRepository<ExternalMedicalPost>
{

    // Opcionales, útiles para reglas de negocio
    Task<ExternalMedicalPost?> GetByNameAsync(string name);
}
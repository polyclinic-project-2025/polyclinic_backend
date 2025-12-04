using PolyclinicDomain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PolyclinicDomain.IRepositories{

    public interface IDerivationRepository : IRepository<Derivation>
{
    // Buscar por nombre del Departamento origen
    Task<IEnumerable<Derivation>> GetByDepartmentFromNameAsync(string departmentName);

    // Buscar por nombre del Departamento destino
    Task<IEnumerable<Derivation>> GetByDepartmentToNameAsync(string departmentName);

    // Buscar por fecha exacta
    Task<IEnumerable<Derivation>> GetByDateAsync(DateTime date);

    // Buscar por nombre del paciente
    Task<IEnumerable<Derivation>> GetByPatientNameAsync(string patientName);
    //Buscar por Identificacion del paciente
    Task<IEnumerable<Derivation>> GetByPatientIdentificationAsync(string patientIdentification);
    Task<Derivation> GetByIdWithIncludesAsync(Guid id);
    Task<IEnumerable<Derivation>> GetAllWithIncludesAsync();
}
}


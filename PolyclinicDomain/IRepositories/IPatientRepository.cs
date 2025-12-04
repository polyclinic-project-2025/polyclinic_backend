using PolyclinicDomain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PolyclinicDomain.IRepositories
{
    public interface IPatientRepository : IRepository<Patient>
    {
        // Buscar por campos específicos
        Task<IEnumerable<Patient>> GetByNameAsync(string name);
        Task<Patient?> GetByIdentificationAsync(string identification);
        Task<IEnumerable<Patient>> GetByAgeAsync(int age);

        //Método para traer pacientes con todas sus relaciones
        Task<Patient?> GetPatientWithAllRelationsAsync(Guid patientId);
    }
}

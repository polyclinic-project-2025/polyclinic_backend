using PolyclinicDomain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PolyclinicDomain.IRepositories
{
    public interface IReferralRepository : IRepository<Referral>{
         // Buscar por nombre del Puesto Externo
    Task<IEnumerable<Referral>> GetByPuestoExternoAsync(string PuestoExterno);

    // Buscar por nombre del Departamento destino
    Task<IEnumerable<Referral>> GetByDepartmentToNameAsync(string departmentName);

    // Buscar por fecha exacta
    Task<IEnumerable<Referral>> GetByDateAsync(DateTime date);

    // Buscar por nombre del paciente
    Task<IEnumerable<Referral>> GetByPatientNameAsync(string patientName);
    //Buscar por Identificacion del paciente
    Task<IEnumerable<Referral>> GetByPatientIdentificationAsync(string patientIdentification);
    Task<Referral> GetByIdWithIncludesAsync(Guid id);
    Task<IEnumerable<Referral>> GetAllWithIncludesAsync();
    }

}
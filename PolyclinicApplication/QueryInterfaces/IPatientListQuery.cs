using System.Collections.Generic;
using System.Threading.Tasks;
using PolyclinicApplication.ReadModels;

namespace PolyclinicApplication.QueryInterfaces;

public interface IPatientListQuery
{
    Task<IEnumerable<PatientListReadModel>> GetPatientsListAsync();
}
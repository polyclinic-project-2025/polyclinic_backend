using System.Collections.Generic;
using System.Threading.Tasks;
using PolyclinicApplication.Common.Results;
using PolyclinicApplication.ReadModels;

namespace PolyclinicApplication.Services.Interfaces.Analytics;

public interface IPatientListService
{
    Task<Result<IEnumerable<PatientListReadModel>>> GetPatientsListAsync();
}
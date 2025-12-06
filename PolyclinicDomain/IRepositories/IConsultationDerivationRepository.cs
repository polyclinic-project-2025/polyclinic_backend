using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PolyclinicDomain.Entities;

namespace PolyclinicDomain.IRepositories;

public interface IConsultationDerivationRepository : IRepository<ConsultationDerivation>
{
    Task<IEnumerable<ConsultationDerivation>> GetByDateRangeAsync(Guid patientId,DateTime startDate,DateTime endDate);

    Task<IEnumerable<ConsultationDerivation>> GetLast10ByPatientIdAsync(Guid patientId);
}

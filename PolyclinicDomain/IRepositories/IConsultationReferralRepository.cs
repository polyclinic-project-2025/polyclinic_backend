namespace PolyclinicDomain.IRepositories
{
    using PolyclinicDomain.Entities;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IConsultationReferralRepository : IRepository<ConsultationReferral>
    {
        public Task<ConsultationReferral?> GetByIdWithDeepIncludesAsync(Guid id);

        Task<IEnumerable<ConsultationReferral>> GetByDateRangeAsync(Guid patientId, DateTime startDate, DateTime endDate);
        Task<IEnumerable<ConsultationReferral>> GetLast10ByPatientIdAsync(Guid patientId);
        Task<ConsultationReferral?> GetWithDepartmentAsync(Guid consultationReferralId);
    }
}
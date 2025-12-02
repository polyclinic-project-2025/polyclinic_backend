namespace PolyclinicDomain.IRepositories
{
    using PolyclinicDomain.Entities;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IConsultationReferralRepository : IRepository<ConsultationReferral>
    {
        public Task<ConsultationReferral?> GetByIdWithDeepIncludesAsync(Guid id);
    }
}
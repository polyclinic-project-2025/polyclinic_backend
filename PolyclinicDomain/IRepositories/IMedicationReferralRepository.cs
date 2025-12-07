using PolyclinicDomain.Entities;

namespace PolyclinicDomain.IRepositories;

public interface IMedicationReferralRepository : IRepository<MedicationReferral>
{
    // Métodos específicos si se necesitan en el futuro
    Task<IEnumerable<MedicationReferral>> GetByConsultationReferralIdAsync(Guid consultationReferralId);
    Task<IEnumerable<MedicationReferral>> GetByMedicationIdAsync(Guid medicationId);
}

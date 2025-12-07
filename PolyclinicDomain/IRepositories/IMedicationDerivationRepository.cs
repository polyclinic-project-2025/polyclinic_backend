using PolyclinicDomain.Entities;

namespace PolyclinicDomain.IRepositories;

public interface IMedicationDerivationRepository : IRepository<MedicationDerivation>
{
    // Métodos específicos si se necesitan en el futuro
    Task<IEnumerable<MedicationDerivation>> GetByConsultationDerivationIdAsync(Guid consultationDerivationId);
    Task<IEnumerable<MedicationDerivation>> GetByMedicationIdAsync(Guid medicationId);
}
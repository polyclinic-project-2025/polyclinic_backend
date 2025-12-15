using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PolyclinicDomain.Entities;
using PolyclinicApplication.QueryInterfaces;
using PolyclinicApplication.ReadModels;
using PolyclinicInfrastructure.Persistence;

namespace PolyclinicInfrastructure.Queries;

public class MedicationConsumptionQuery : IMedicationConsumptionQuery
{
    private readonly AppDbContext _context;

    public MedicationConsumptionQuery(AppDbContext context)
    {
        _context = context;
    }

    public async Task<MedicationConsumptionReadModel> GetMonthlyConsumptionAsync(
        Guid medicationId, 
        int month, 
        int year)
    {
        // 1. Consumo de MedicationDerivation
        var derivationConsumption = await _context.Set<MedicationDerivation>()
            .Where(md => md.MedicationId == medicationId &&
                         md.ConsultationDerivation!.DateTimeCDer.Month == month &&
                         md.ConsultationDerivation.DateTimeCDer.Year == year)
            .SumAsync(md => md.Quantity);

        // 2. Consumo de MedicationReferral
        var referralConsumption = await _context.Set<MedicationReferral>()
            .Where(mr => mr.MedicationId == medicationId &&
                         mr.ConsultationReferral!.DateTimeCRem.Month == month &&
                         mr.ConsultationReferral.DateTimeCRem.Year == year)
            .SumAsync(mr => mr.Quantity);

        // 3. Consumo de MedicationEmergency
        var emergencyConsumption = await _context.Set<MedicationEmergency>()
            .Where(me => me.MedicationId == medicationId &&
                         me.EmergencyRoomCare!.CareDate.Month == month &&
                         me.EmergencyRoomCare.CareDate.Year == year)
            .SumAsync(me => me.Quantity);

        // 4. Suma total de consumos
        var totalConsumption = derivationConsumption + referralConsumption + emergencyConsumption;

        // 5. Obtener información del medicamento y crear el ReadModel
        var medication = await _context.Set<Medication>()
            .Where(m => m.MedicationId == medicationId)
            .FirstAsync();
        
        return new MedicationConsumptionReadModel(
            Month: month,
            Year: year,
            ScientificName: medication.ScientificName,
            CommercialName: medication.CommercialName,
            TotalConsumption: totalConsumption,
            QuantityWarehouse: medication.QuantityWarehouse,
            MinQuantityWarehouse: medication.MinQuantityWarehouse,
            MaxQuantityWarehouse: medication.MaxQuantityWarehouse
        );
    }
}
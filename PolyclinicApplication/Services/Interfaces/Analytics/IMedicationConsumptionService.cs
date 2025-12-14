using System;
using System.Threading.Tasks;
using PolyclinicApplication.Common.Results;
using PolyclinicApplication.ReadModels;

namespace PolyclinicApplication.Services.Interfaces.Analytics;

public interface IMedicationConsumptionService
{
    Task<Result<MedicationConsumptionReadModel>> GetMonthlyConsumptionAsync(
        Guid medicationId, 
        int month, 
        int year);
}
using System;
using System.Threading.Tasks;
using PolyclinicApplication.ReadModels;

namespace PolyclinicApplication.QueryInterfaces;

public interface IMedicationConsumptionQuery
{
    Task<MedicationConsumptionReadModel> GetMonthlyConsumptionAsync(
        Guid medicationId, 
        int month, 
        int year);
}
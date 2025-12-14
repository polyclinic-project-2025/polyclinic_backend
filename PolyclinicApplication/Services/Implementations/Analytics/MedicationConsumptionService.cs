using System;
using System.Threading.Tasks;
using PolyclinicApplication.Common.Results;
using PolyclinicApplication.QueryInterfaces;
using PolyclinicApplication.ReadModels;
using PolyclinicApplication.Services.Interfaces.Analytics;

namespace PolyclinicApplication.Services.Implementations.Analytics;

public class MedicationConsumptionService : IMedicationConsumptionService
{
    private readonly IMedicationConsumptionQuery _query;

    public MedicationConsumptionService(IMedicationConsumptionQuery query)
    {
        _query = query;
    }

    public async Task<Result<MedicationConsumptionReadModel>> GetMonthlyConsumptionAsync(
        Guid medicationId, 
        int month, 
        int year)
    {
        // Validaciones
        if (medicationId == Guid.Empty)
            return Result<MedicationConsumptionReadModel>.Failure("El ID del medicamento no es válido");

        if (month < 1 || month > 12)
            return Result<MedicationConsumptionReadModel>.Failure("El mes debe estar entre 1 y 12");

        if (year < 1900 || year > DateTime.Now.Year)
            return Result<MedicationConsumptionReadModel>.Failure($"El año debe estar entre 1900 y {DateTime.Now.Year}");

        try
        {
            var result = await _query.GetMonthlyConsumptionAsync(medicationId, month, year);
            
            if (result == null)
                return Result<MedicationConsumptionReadModel>.Failure("No se encontró información de consumo para los parámetros especificados");

            return Result<MedicationConsumptionReadModel>.Success(result);
        }
        catch (Exception ex)
        {
            return Result<MedicationConsumptionReadModel>.Failure($"Error al obtener el consumo: {ex.Message}");
        }
    }
}
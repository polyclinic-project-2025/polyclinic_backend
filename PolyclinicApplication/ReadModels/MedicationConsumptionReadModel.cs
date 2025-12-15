using System;
using System.Globalization;

namespace PolyclinicApplication.ReadModels;

public record MedicationConsumptionReadModel(
    int Month,
    int Year,
    string ScientificName,
    string CommercialName,
    int TotalConsumption,
    int QuantityWarehouse,
    int MinQuantityWarehouse,
    int MaxQuantityWarehouse
)
{
    // Propiedad calculada para el nombre del mes
    public string MonthName => new DateTime(Year, Month, 1)
        .ToString("MMMM", new CultureInfo("es-ES"));
    
    // O si prefieres capitalizar la primera letra
    public string MonthNameCapitalized 
    {
        get
        {
            var monthName = new DateTime(Year, Month, 1)
                .ToString("MMMM", new CultureInfo("es-ES"));
            return char.ToUpper(monthName[0]) + monthName.Substring(1);
        }
    }
}
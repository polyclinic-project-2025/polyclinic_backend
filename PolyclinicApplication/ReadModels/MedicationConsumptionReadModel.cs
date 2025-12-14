using System;

namespace PolyclinicApplication.ReadModels;

public record MedicationConsumptionReadModel(
    string ScientificName,
    string CommercialName,
    int TotalConsumption,
    int QuantityWarehouse,
    int MinQuantityWarehouse,
    int MaxQuantityWarehouse
);
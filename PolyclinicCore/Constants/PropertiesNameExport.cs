namespace PolyclinicCore.Constants;

public static class PropertiesNameExport
{
    public static Dictionary<string, string> Properties { get; set; } = new Dictionary<string, string>
    {
        { "Address", "Dirección" },
        { "Age", "Edad" },
        { "BatchNumber", "Número de Lote" },
        { "CommercialCompany", "Compañía Comercial" },
        { "CommercialName", "Nombre Comercial" },
        { "ConsultationDate", "Fecha" },
        { "ConsultationAverage", "Atenciones Consultas" },
        { "Contact", "Contacto" },
        { "DepartmentName", "Departamento" },
        { "DepartmentHeadName", "Jefe de Departamento" },
        { "Diagnosis", "Diagnóstico" },
        { "Doctor", "Doctor" },
        { "DoctorFullName", "Doctor"},
        { "DoctorName", "Doctor"},
        { "EmergencyRoomAverage", "Atenciones Urgencia" },
        { "ExpirationDate", "Fecha de Expiración" },
        { "Format", "Formato" },
        { "FrequentMedications", "Medicamentos Frecuentes" },
        { "Identification", "Identificación" },
        { "Medications", "Medicamentos" },
        { "Name", "Nombre" },
        { "PatientFullName", "Paciente" },
        { "ScientificName", "Nombre Científico" },
        { "SuccessRate", "Tasa de Éxito" },
        { "TotalPrescriptions", "Prescripciones" },
        { "Type", "Tipo" },
        { "Quantity", "Cantidad" },
        { "QuantityNurse", "Cantidad en Enfermería" },
        { "QuantityWarehouse", "Cantidad en Almacén" }
    };
}
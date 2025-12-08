using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PolyclinicApplication.DTOs.Response{
    public class StockDepartmentDto
    {
        public Guid StockDepartmentId { get; set; }
        public int Quantity { get; set; }
        public Guid DepartmentId { get; set; }
        public Guid MedicationId { get; set; }

        // Información del medicamento
        public string MedicationCommercialName { get; set; } = string.Empty;
        public string MedicationScientificName { get; set; } = string.Empty;

        public int MinQuantity { get; set; }
        public int MaxQuantity { get; set; }
    }
}
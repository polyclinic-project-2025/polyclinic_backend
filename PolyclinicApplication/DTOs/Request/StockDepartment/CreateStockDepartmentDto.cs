using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PolyclinicApplication.DTOs.Request.StockDepartment
{
    public class CreateStockDepartmentDto
    {
        public int Quantity { get; set; }
        public Guid DepartmentId { get; set; }
        public Guid MedicationId { get; set; }
        public int MinQuantity { get; set; }
        public int MaxQuantity { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PolyclinicApplication.DTOs.Request;

public class CreateMedicationRequestRequest
{
    public int Quantity { get; set; }
    public Guid WarehouseRequestId { get; set; }
    public Guid MedicationId { get; set; }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PolyclinicApplication.DTOs.Response;

public class MedicationRequestResponse
{
    public Guid MedicationRequestId { get; set; }
    public int Quantity { get; set; }
    public Guid WarehouseRequestId { get; set; }
    public Guid MedicationId { get; set; }
}
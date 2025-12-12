using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PolyclinicApplication.DTOs.Response;

public class WarehouseRequestResponse
{
    public Guid WarehouseRequestId { get; set; }
    public string Status { get; set; }
    public DateTime RequestDate { get; set; }
    public Guid DepartmentId { get; set; }
    public Guid WarehouseManagerId { get; set; }
}

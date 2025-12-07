using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PolyclinicApplication.DTOs.Response;

public class WarehouseManagerResponse : EmployeeResponse
{
    public DateTime AssignedAt { get; set; }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PolyclinicApplication.DTOs.Request;

public class UpdateEmployeeRequest
{
    public string? Identification { get; set; }
    public string? Name { get; set; }
    public string? EmploymentStatus { get; set; }
}
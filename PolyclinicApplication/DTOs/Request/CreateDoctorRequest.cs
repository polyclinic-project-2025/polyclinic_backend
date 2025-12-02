using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PolyclinicApplication.DTOs.Request;

public class CreateDoctorRequest : CreateEmployeeRequest
{
    public Guid DepartmentId { get; set; }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PolyclinicApplication.DTOs.Request;

public class AssignDepartmentHeadRequest
{
    public Guid DoctorId { get; set; }
}
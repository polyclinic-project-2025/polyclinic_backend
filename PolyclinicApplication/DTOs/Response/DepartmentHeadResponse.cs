using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PolyclinicApplication.DTOs.Response;

public class DepartmentHeadResponse
{
    public Guid DepartmentHeadId { get; set; }
    public Guid DoctorId { get; set; }
    public Guid DepartmentId { get; set; }
    public DateTime AssignedAt { get; set; }
}
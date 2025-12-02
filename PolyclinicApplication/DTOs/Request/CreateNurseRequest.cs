using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PolyclinicApplication.DTOs.Request;

public class CreateNurseRequest : CreateEmployeeRequest
{
    public Guid NursingId { get; set; }
}
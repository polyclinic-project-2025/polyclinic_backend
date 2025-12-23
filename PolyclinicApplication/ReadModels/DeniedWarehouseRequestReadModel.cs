using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PolyclinicApplication.ReadModels;

public record DeniedWarehouseRequestReadModel(
    string DepartmentName,
    string DepartmentHeadName,
    string Medications
);
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PolyclinicApplication.ReadModels;

public record DeniedWarehouseRequestReadModel(
    string Status,
    string DepartmentName,
    string DepartmentHeadName
);
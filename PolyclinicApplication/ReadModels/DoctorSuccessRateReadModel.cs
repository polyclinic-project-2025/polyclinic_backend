using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PolyclinicApplication.ReadModels;

public record DoctorSuccessRateReadModel(
    string DoctorName,
    string DepartmentName,
    double SuccessRate,
    int TotalPrescriptions,
    string FrequentMedications
);
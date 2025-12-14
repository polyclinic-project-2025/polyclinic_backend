using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PolyclinicApplication.ReadModels;

public record DoctorMonthlyAverageReadModel(
    string DoctorName,
    string DepartmentName,
    double ConsultationAverage,
    double EmergencyRoomAverage
);
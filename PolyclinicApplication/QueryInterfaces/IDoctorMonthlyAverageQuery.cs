using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PolyclinicApplication.ReadModels;

namespace PolyclinicApplication.QueryInterfaces;

public interface IDoctorMonthlyAverageQuery
{
    Task<IEnumerable<DoctorMonthlyAverageReadModel>> GetDoctorAverageAsync(DateTime from, DateTime to);
}
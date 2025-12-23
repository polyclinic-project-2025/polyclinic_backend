using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PolyclinicDomain.Entities;
using PolyclinicApplication.QueryInterfaces;
using PolyclinicApplication.ReadModels;
using PolyclinicInfrastructure.Persistence;

namespace PolyclinicInfrastructure.Queries;

public class PatientListQuery : IPatientListQuery
{
    private readonly DbSet<Patient> _dbSetPatient;

    public PatientListQuery(AppDbContext context)
    {
        _dbSetPatient = context.Set<Patient>();
    }

    public async Task<IEnumerable<PatientListReadModel>> GetPatientsListAsync()
    {
        var patients = await _dbSetPatient
            .OrderBy(p => p.Name)
            .Select(p => new PatientListReadModel(
                p.Name,                    // PatientName - posición 1
                p.Identification,          // Identification - posición 2
                p.Age,                     // Age - posición 3
                p.Contact ?? "No especificado", // Contact - posición 4
                p.Address                  // Address - posición 5
            ))
            .ToListAsync();

        return patients;
    }
}
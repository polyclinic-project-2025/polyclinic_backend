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

public class DeniedWarehouseRequestsQuery : IDeniedWarehouseRequestsQuery
{
    private readonly DbSet<WarehouseRequest> _dbSet;

    public DeniedWarehouseRequestsQuery(AppDbContext context)
    {
        _dbSet = context.Set<WarehouseRequest>();
    }

    public async Task<IEnumerable<DeniedWarehouseRequestReadModel>> GetDeniedAsync(string status)
        => await _dbSet.Where(wr => wr.Status == status)
                        .Select(wr => new DeniedWarehouseRequestReadModel(
                            wr.Department.Name,
                            wr.Department.DepartmentHeads
                                .OrderByDescending(dh => dh.AssignedAt)
                                .Select(dh => dh.Doctor.Name)
                                .FirstOrDefault(),
                            string.Join(
                                ", ", 
                                wr.MedicationRequests
                                    .Select(mr => $"{mr.Medication.CommercialName} ({mr.Quantity} u)")
                            ) + "."
                        ))
                        .ToListAsync();
}

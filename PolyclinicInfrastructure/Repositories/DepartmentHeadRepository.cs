using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PolyclinicDomain.Entities;
using PolyclinicDomain.IRepositories;

namespace PolyclinicInfrastructure.Repositories;

public class DepartmentHeadRepository : Repository<DepartmentHead>, IDepartmentHeadRepository
{
    private readonly DbSet<DepartmentHead> _dbSet;

    public DepartmentHeadRepository(AppDbContext context) : base(context)
    {
        _dbSet = _context.Set<DepartmentHead>();
    }

    public async Task<DepartmentHead?> GetByDepartmentIdAsync(Guid departmentId)
    {
        return await _dbSet.Include(dh => dh.Doctor)
                           .Where(dh => dh.Doctor != null && dh.Doctor.DepartmentId == departmentId)
                           .FirstOrDefaultAsync();
    }

    public async Task<DepartmentHead?> GetByIdentificationAsync(string identification)
    {
        return await _dbSet.Include(dh => dh.Doctor)
                           .Where(dh => dh.Doctor != null && dh.Doctor.Identification == identification)
                           .FirstOrDefaultAsync();
    }
}
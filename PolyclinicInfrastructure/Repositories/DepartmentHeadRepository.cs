using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PolyclinicDomain.Entities;
using PolyclinicDomain.IRepositories;
using PolyclinicInfrastructure.Persistence;
namespace PolyclinicInfrastructure.Repositories;

public class DepartmentHeadRepository : Repository<DepartmentHead>, IDepartmentHeadRepository
{
    private readonly DbSet<DepartmentHead> _dbSet;

    public DepartmentHeadRepository(AppDbContext context) : base(context)
    {
        _dbSet = _context.Set<DepartmentHead>();
    }

    public async Task<DepartmentHead?> GetByDepartmentIdAsync(Guid departmentId)
        => await _dbSet.Include(dh => dh.Doctor)
                            .Where(dh => dh.DepartmentId == departmentId && dh.Doctor.DepartmentId == departmentId)
                            .OrderByDescending(dh => dh.AssignedAt)
                            .FirstOrDefaultAsync();

    public async Task<DepartmentHead?> GetByIdentificationAsync(string identification)
        => await _dbSet.Include(dh => dh.Doctor)
                            .Where(dh => dh.Doctor != null && dh.Doctor.Identification == identification)
                            .FirstOrDefaultAsync();
}
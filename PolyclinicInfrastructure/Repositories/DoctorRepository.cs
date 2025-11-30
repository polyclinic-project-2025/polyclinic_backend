using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PolyclinicDomain.Entities;
using PolyclinicDomain.IRepositories;
using PolyclinicInfrastructure.Persistence;

namespace PolyclinicInfrastructure.Repositories;

public class DoctorRepository : Repository<Doctor>, IDoctorRepository
{
    private readonly DbSet<Doctor> _dbSet;

    public DoctorRepository(AppDbContext context) : base(context)
    {
        _dbSet = _context.Set<Doctor>();
    }

    public async Task<Doctor?> GetByIdentificationAsync(string identification)
        => await _dbSet.FirstOrDefaultAsync(d => d.Identification == identification);

    public async Task<bool> ExistsByIdentificationAsync(string identification)
        => await _dbSet.AnyAsync(d => d.Identification == identification);
}
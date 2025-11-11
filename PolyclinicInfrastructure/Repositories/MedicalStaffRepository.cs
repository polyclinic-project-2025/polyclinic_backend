using Microsoft.EntityFrameworkCore;
using PolyclinicDomain.Entities;
using PolyclinicDomain.IRepositories;
using PolyclinicInfrastructure.Persistence;

namespace PolyclinicInfrastructure.Repositories;

public class MedicalStaffRepository : Repository<MedicalStaff>, IMedicalStaff
{
    public MedicalStaffRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<MedicalStaff>> GetByDepartmentAsync(Guid departmentId)
        => await _context.Set<MedicalStaff>()
            .Where(m => m.DepartmentId == departmentId)
            .ToListAsync();
}

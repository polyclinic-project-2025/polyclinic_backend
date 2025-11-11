using Microsoft.EntityFrameworkCore;
using PolyclinicDomain.Entities;
using PolyclinicDomain.IRepositories;
using PolyclinicInfrastructure.Persistence;

namespace PolyclinicInfrastructure.Repositories;

public class EmployeeRepository : Repository<Employee>, IEmployeeRepository
{
    public EmployeeRepository(AppDbContext context) : base(context) { }

    public async Task<Employee?> GetByIdentificationAsync(string identification)
        => await _context.Set<Employee>()
            .FirstOrDefaultAsync(e => e.Identification == identification);

    public async Task<IEnumerable<Employee>> GetByNameAsync(string name)
        => await _context.Set<Employee>()
            .Where(e => e.Name.Contains(name))
            .ToListAsync();

    public async Task<IEnumerable<Employee>> GetByEmploymentStatusAsync(string status)
        => await _context.Set<Employee>()
            .Where(e => e.EmploymentStatus == status)
            .ToListAsync();

    public async Task<IEnumerable<Employee>> GetByPrimaryRoleAsync(string role)
    {
        var employees = await _context.Set<Employee>().ToListAsync();
        return employees.Where(e => e.GetPrimaryRole() == role);
    }
}
 
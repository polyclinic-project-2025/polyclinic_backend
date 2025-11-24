using Microsoft.EntityFrameworkCore;
using PolyclinicDomain.Entities;
using PolyclinicDomain.IRepositories;
using PolyclinicInfrastructure.Persistence;
using System.Threading.Tasks;

namespace PolyclinicInfrastructure.Repositories
{
    public class EmployeeRepository : Repository<Employee>, IEmployeeRepository
    {
        private readonly AppDbContext _context;

        public EmployeeRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Employee> GetByIdentificationAsync(string identification)
        {
            return await _context.Employees
                .FirstOrDefaultAsync(e => e.Identification == identification);
        }

        public async Task<Employee> GetByNameAsync(string name)
        {
            return await _context.Employees
                .FirstOrDefaultAsync(e => e.Name == name);
        }

        public async Task<Employee> GetByEmploymentStatusAsync(string employmentStatus)
        {
            return await _context.Employees
                .FirstOrDefaultAsync(e => e.EmploymentStatus == employmentStatus);
        }

        public async Task<Employee> GetByPrimaryRoleAsync(string primaryRole)
        {
            return await _context.Employees
                .FirstOrDefaultAsync(e => e.GetPrimaryRole() == primaryRole);
        }

    }
}

using Microsoft.EntityFrameworkCore;
using PolyclinicDomain.Entities;
using PolyclinicDomain.IRepositories;
using PolyclinicInfrastructure.Persistence;


namespace PolyclinicInfrastructure.Repositories
{
    public class StockDepartmentRepository : Repository<StockDepartment>, IStockDepartmentRepository
    {
        private readonly AppDbContext _context;

        public StockDepartmentRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<IEnumerable<StockDepartment>> GetStockByDepartmentAsync(Guid departmentId)
        {
            return await _context.StockDepartments
                .Where(sd => sd.DepartmentId == departmentId)
                .Include(sd => sd.Medication)
                .ToListAsync();
        }

        public async Task<IEnumerable<StockDepartment>> GetBelowMinQuantityAsync(Guid departmentId)
        {
            return await _context.StockDepartments
                .Where(sd => sd.DepartmentId == departmentId && sd.Quantity < sd.MinQuantity)
                .Include(sd => sd.Medication)
                .ToListAsync();
        }

        public async Task<IEnumerable<StockDepartment>> GetAboveMaxQuantityAsync(Guid departmentId)
        {
            return await _context.StockDepartments
                .Where(sd => sd.DepartmentId == departmentId && sd.Quantity > sd.MaxQuantity)
                .Include(sd => sd.Medication)
                .ToListAsync();
        }

        // ✨ NUEVO MÉTODO
        public async Task<StockDepartment?> GetByDepartmentAndMedicationAsync(Guid departmentId, Guid medicationId)
        {
            return await _context.StockDepartments
                .FirstOrDefaultAsync(sd => sd.DepartmentId == departmentId && sd.MedicationId == medicationId);
        }
    }
}
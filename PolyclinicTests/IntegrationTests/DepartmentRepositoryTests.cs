using Xunit;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using FluentAssertions;
using PolyclinicDomain.Entities;
using PolyclinicInfrastructure.Persistence;
using PolyclinicInfrastructure.Repositories;

namespace PolyclinicTests.IntegrationTests
{
    public class DepartmentRepositoryTests
    {
        private readonly AppDbContext _context;
        private readonly DepartmentRepository _repository;

        public DepartmentRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase("PolyclinicTestDb")
                .Options;

            _context = new AppDbContext(options);
            _repository = new DepartmentRepository(_context);
        }

        [Fact]
        public async Task AddAsync_ShouldPersistDepartment()
        {
            var department = new Department(Guid.NewGuid(), "Neurology", null);

            await _repository.AddAsync(department);
            var result = await _repository.GetByNameAsync("Neurology");

            result.Should().NotBeNull();
            result!.Name.Should().Be("Neurology");
        }

        [Fact]
        public async Task ExistsByNameAsync_ShouldReturnTrue_WhenExists()
        {
            var department = new Department(Guid.NewGuid(), "Radiology", null);
            await _repository.AddAsync(department);

            var exists = await _repository.ExistsByNameAsync("Radiology");
            exists.Should().BeTrue();
        }
    }
}

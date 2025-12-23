using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Moq;
using AutoMapper;
using PolyclinicApplication.Services.Implementations;
using PolyclinicApplication.Services.Interfaces;
using PolyclinicApplication.DTOs.Departments;
using PolyclinicApplication.DTOs.Response;
using PolyclinicDomain.Entities;
using PolyclinicDomain.IRepositories;
using PolyclinicApplication.Common.Results;

namespace PolyclinicApplication.Tests.Services
{
    public class DepartmentServiceTests
    {
        private readonly Mock<IDepartmentRepository> _mockRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly DepartmentService _departmentService;

        public DepartmentServiceTests()
        {
            _mockRepository = new Mock<IDepartmentRepository>();
            _mockMapper = new Mock<IMapper>();
            _departmentService = new DepartmentService(_mockRepository.Object, _mockMapper.Object);
        }

        #region CreateAsync Tests

        [Fact]
        public async Task CreateAsync_ValidDepartment_ReturnsSuccessResult()
        {
            // ARRANGE
            var createDto = new CreateDepartmentDto { Name = "Cardiología" };
            var department = new Department(Guid.NewGuid(), createDto.Name);
            var departmentDto = new DepartmentDto { DepartmentId = department.DepartmentId, Name = createDto.Name };

            _mockRepository.Setup(r => r.ExistsByNameAsync(createDto.Name)).ReturnsAsync(false);
            _mockRepository.Setup(r => r.AddAsync(It.IsAny<Department>())).ReturnsAsync(department);
            _mockMapper.Setup(m => m.Map<DepartmentDto>(It.IsAny<Department>())).Returns(departmentDto);

            // ACT
            var result = await _departmentService.CreateAsync(createDto);

            // ASSERT
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(createDto.Name, result.Value.Name);
            _mockRepository.Verify(r => r.ExistsByNameAsync(createDto.Name), Times.Once);
            _mockRepository.Verify(r => r.AddAsync(It.IsAny<Department>()), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_DuplicateName_ReturnsFailureResult()
        {
            // ARRANGE
            var createDto = new CreateDepartmentDto { Name = "Cardiología" };
            _mockRepository.Setup(r => r.ExistsByNameAsync(createDto.Name)).ReturnsAsync(true);

            // ACT
            var result = await _departmentService.CreateAsync(createDto);

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("Ya existe un departamento", result.ErrorMessage);
            Assert.Contains(createDto.Name, result.ErrorMessage);
            _mockRepository.Verify(r => r.AddAsync(It.IsAny<Department>()), Times.Never);
        }

        [Fact]
        public async Task CreateAsync_RepositoryThrowsException_ReturnsFailureResult()
        {
            // ARRANGE
            var createDto = new CreateDepartmentDto { Name = "Cardiología" };
            _mockRepository.Setup(r => r.ExistsByNameAsync(createDto.Name)).ReturnsAsync(false);
            _mockRepository.Setup(r => r.AddAsync(It.IsAny<Department>())).ThrowsAsync(new Exception("Database error"));

            // ACT
            var result = await _departmentService.CreateAsync(createDto);

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("Error al guardar el departamento", result.ErrorMessage);
        }

        #endregion

        #region GetAllAsync Tests

        [Fact]
        public async Task GetAllAsync_DepartmentsExist_ReturnsSuccessWithList()
        {
            // ARRANGE
            var departments = new List<Department>
            {
                new Department(Guid.NewGuid(), "Cardiología"),
                new Department(Guid.NewGuid(), "Neurología"),
                new Department(Guid.NewGuid(), "Pediatría")
            };

            var departmentDtos = new List<DepartmentDto>
            {
                new DepartmentDto { DepartmentId = departments[0].DepartmentId, Name = "Cardiología" },
                new DepartmentDto { DepartmentId = departments[1].DepartmentId, Name = "Neurología" },
                new DepartmentDto { DepartmentId = departments[2].DepartmentId, Name = "Pediatría" }
            };

            _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(departments);
            _mockMapper.Setup(m => m.Map<IEnumerable<DepartmentDto>>(departments)).Returns(departmentDtos);

            // ACT
            var result = await _departmentService.GetAllAsync();

            // ASSERT
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(3, result.Value.Count());
            _mockRepository.Verify(r => r.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetAllAsync_NoDepartments_ReturnsSuccessWithEmptyList()
        {
            // ARRANGE
            var emptyList = new List<Department>();
            _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(emptyList);
            _mockMapper.Setup(m => m.Map<IEnumerable<DepartmentDto>>(emptyList)).Returns(new List<DepartmentDto>());

            // ACT
            var result = await _departmentService.GetAllAsync();

            // ASSERT
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Empty(result.Value);
        }

        [Fact]
        public async Task GetAllAsync_RepositoryThrowsException_ReturnsFailureResult()
        {
            // ARRANGE
            _mockRepository.Setup(r => r.GetAllAsync()).ThrowsAsync(new Exception("Database error"));

            // ACT
            var result = await _departmentService.GetAllAsync();

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("Error al obtener departamentos", result.ErrorMessage);
        }

        #endregion

        #region GetByIdAsync Tests

        [Fact]
        public async Task GetByIdAsync_ValidId_ReturnsSuccessWithDepartment()
        {
            // ARRANGE
            var departmentId = Guid.NewGuid();
            var department = new Department(departmentId, "Cardiología");
            var departmentDto = new DepartmentDto { DepartmentId = departmentId, Name = "Cardiología" };

            _mockRepository.Setup(r => r.GetWithHeadAsync(departmentId)).ReturnsAsync(department);
            _mockMapper.Setup(m => m.Map<DepartmentDto>(department)).Returns(departmentDto);

            // ACT
            var result = await _departmentService.GetByIdAsync(departmentId);

            // ASSERT
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(departmentId, result.Value.DepartmentId);
            Assert.Equal("Cardiología", result.Value.Name);
            _mockRepository.Verify(r => r.GetWithHeadAsync(departmentId), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_DepartmentNotFound_ReturnsFailureResult()
        {
            // ARRANGE
            var departmentId = Guid.NewGuid();
            _mockRepository.Setup(r => r.GetWithHeadAsync(departmentId)).ReturnsAsync((Department?)null);

            // ACT
            var result = await _departmentService.GetByIdAsync(departmentId);

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("Departamento no encontrado", result.ErrorMessage);
        }

        [Fact]
        public async Task GetByIdAsync_RepositoryThrowsException_ReturnsFailureResult()
        {
            // ARRANGE
            var departmentId = Guid.NewGuid();
            _mockRepository.Setup(r => r.GetWithHeadAsync(departmentId)).ThrowsAsync(new Exception("Database error"));

            // ACT
            var result = await _departmentService.GetByIdAsync(departmentId);

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("Error al obtener departamento", result.ErrorMessage);
        }

        #endregion

        #region UpdateAsync Tests

        [Fact]
        public async Task UpdateAsync_ValidUpdate_ReturnsSuccessResult()
        {
            // ARRANGE
            var departmentId = Guid.NewGuid();
            var existingDepartment = new Department(departmentId, "Cardiología");
            var updateDto = new UpdateDepartmentDto { Name = "Cardiología Avanzada" };

            _mockRepository.Setup(r => r.GetByIdAsync(departmentId)).ReturnsAsync(existingDepartment);
            _mockRepository.Setup(r => r.ExistsByNameAsync(updateDto.Name)).ReturnsAsync(false);
            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Department>())).Returns(Task.CompletedTask);

            // ACT
            var result = await _departmentService.UpdateAsync(departmentId, updateDto);

            // ASSERT
            Assert.True(result.IsSuccess);
            Assert.True(result.Value);
            _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Department>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_DepartmentNotFound_ReturnsFailureResult()
        {
            // ARRANGE
            var departmentId = Guid.NewGuid();
            var updateDto = new UpdateDepartmentDto { Name = "Nueva Cardiología" };
            _mockRepository.Setup(r => r.GetByIdAsync(departmentId)).ReturnsAsync((Department?)null);

            // ACT
            var result = await _departmentService.UpdateAsync(departmentId, updateDto);

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("Departamento no encontrado", result.ErrorMessage);
            _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Department>()), Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_DuplicateName_ReturnsFailureResult()
        {
            // ARRANGE
            var departmentId = Guid.NewGuid();
            var existingDepartment = new Department(departmentId, "Cardiología");
            var updateDto = new UpdateDepartmentDto { Name = "Neurología" };

            _mockRepository.Setup(r => r.GetByIdAsync(departmentId)).ReturnsAsync(existingDepartment);
            _mockRepository.Setup(r => r.ExistsByNameAsync(updateDto.Name)).ReturnsAsync(true);

            // ACT
            var result = await _departmentService.UpdateAsync(departmentId, updateDto);

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("Ya existe un departamento", result.ErrorMessage);
            Assert.Contains(updateDto.Name, result.ErrorMessage);
            _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Department>()), Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_SameName_SkipsNameValidation()
        {
            // ARRANGE
            var departmentId = Guid.NewGuid();
            var existingDepartment = new Department(departmentId, "Cardiología");
            var updateDto = new UpdateDepartmentDto { Name = "Cardiología" };

            _mockRepository.Setup(r => r.GetByIdAsync(departmentId)).ReturnsAsync(existingDepartment);
            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Department>())).Returns(Task.CompletedTask);

            // ACT
            var result = await _departmentService.UpdateAsync(departmentId, updateDto);

            // ASSERT
            Assert.True(result.IsSuccess);
            _mockRepository.Verify(r => r.ExistsByNameAsync(It.IsAny<string>()), Times.Never);
            _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Department>()), Times.Once);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task UpdateAsync_EmptyOrNullName_SkipsNameUpdate(string emptyName)
        {
            // ARRANGE
            var departmentId = Guid.NewGuid();
            var existingDepartment = new Department(departmentId, "Cardiología");
            var updateDto = new UpdateDepartmentDto { Name = emptyName };

            _mockRepository.Setup(r => r.GetByIdAsync(departmentId)).ReturnsAsync(existingDepartment);
            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Department>())).Returns(Task.CompletedTask);

            // ACT
            var result = await _departmentService.UpdateAsync(departmentId, updateDto);

            // ASSERT
            Assert.True(result.IsSuccess);
            _mockRepository.Verify(r => r.ExistsByNameAsync(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_RepositoryThrowsException_ReturnsFailureResult()
        {
            // ARRANGE
            var departmentId = Guid.NewGuid();
            var existingDepartment = new Department(departmentId, "Cardiología");
            var updateDto = new UpdateDepartmentDto { Name = "Nueva Cardiología" };

            _mockRepository.Setup(r => r.GetByIdAsync(departmentId)).ReturnsAsync(existingDepartment);
            _mockRepository.Setup(r => r.ExistsByNameAsync(updateDto.Name)).ReturnsAsync(false);
            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Department>())).ThrowsAsync(new Exception("Database error"));

            // ACT
            var result = await _departmentService.UpdateAsync(departmentId, updateDto);

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("Error al actualizar el departamento", result.ErrorMessage);
        }

        #endregion

        #region DeleteAsync Tests

        [Fact]
        public async Task DeleteAsync_ValidId_ReturnsSuccessResult()
        {
            // ARRANGE
            var departmentId = Guid.NewGuid();
            var department = new Department(departmentId, "Cardiología");

            _mockRepository.Setup(r => r.GetByIdAsync(departmentId)).ReturnsAsync(department);
            _mockRepository.Setup(r => r.DeleteByIdAsync(departmentId)).Returns(Task.CompletedTask);

            // ACT
            var result = await _departmentService.DeleteAsync(departmentId);

            // ASSERT
            Assert.True(result.IsSuccess);
            Assert.True(result.Value);
            _mockRepository.Verify(r => r.GetByIdAsync(departmentId), Times.Once);
            _mockRepository.Verify(r => r.DeleteByIdAsync(departmentId), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_DepartmentNotFound_ReturnsFailureResult()
        {
            // ARRANGE
            var departmentId = Guid.NewGuid();
            _mockRepository.Setup(r => r.GetByIdAsync(departmentId)).ReturnsAsync((Department?)null);

            // ACT
            var result = await _departmentService.DeleteAsync(departmentId);

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("Departamento no encontrado", result.ErrorMessage);
            _mockRepository.Verify(r => r.DeleteByIdAsync(It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public async Task DeleteAsync_RepositoryThrowsException_ReturnsFailureResult()
        {
            // ARRANGE
            var departmentId = Guid.NewGuid();
            var department = new Department(departmentId, "Cardiología");

            _mockRepository.Setup(r => r.GetByIdAsync(departmentId)).ReturnsAsync(department);
            _mockRepository.Setup(r => r.DeleteByIdAsync(departmentId)).ThrowsAsync(new Exception("Database error"));

            // ACT
            var result = await _departmentService.DeleteAsync(departmentId);

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("Error al eliminar el departamento", result.ErrorMessage);
        }

        #endregion

        #region GetDoctorsByDepartmentIdAsync Tests

        [Fact]
        public async Task GetDoctorsByDepartmentIdAsync_DoctorsExist_ReturnsSuccessWithList()
        {
            // ARRANGE
            var departmentId = Guid.NewGuid();
            var doctors = new List<Doctor>
            {
                new Doctor(Guid.NewGuid(), "12345678", "Dr. Juan Pérez", "Active", departmentId),
                new Doctor(Guid.NewGuid(), "87654321", "Dra. María García", "Active", departmentId)
            };

            var doctorResponses = new List<DoctorResponse>
            {
                new DoctorResponse { EmployeeId = doctors[0].EmployeeId, Name = "Dr. Juan Pérez" },
                new DoctorResponse { EmployeeId = doctors[1].EmployeeId, Name = "Dra. María García" }
            };

            _mockRepository.Setup(r => r.GetDoctorsByDepartmentIdAsync(departmentId)).ReturnsAsync(doctors);
            _mockMapper.Setup(m => m.Map<List<DoctorResponse>>(doctors)).Returns(doctorResponses);

            // ACT
            var result = await _departmentService.GetDoctorsByDepartmentIdAsync(departmentId);

            // ASSERT
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(2, result.Value.Count);
            _mockRepository.Verify(r => r.GetDoctorsByDepartmentIdAsync(departmentId), Times.Once);
        }

        [Fact]
        public async Task GetDoctorsByDepartmentIdAsync_NoDoctors_ReturnsSuccessWithEmptyList()
        {
            // ARRANGE
            var departmentId = Guid.NewGuid();
            var emptyList = new List<Doctor>();

            _mockRepository.Setup(r => r.GetDoctorsByDepartmentIdAsync(departmentId)).ReturnsAsync(emptyList);
            _mockMapper.Setup(m => m.Map<List<DoctorResponse>>(emptyList)).Returns(new List<DoctorResponse>());

            // ACT
            var result = await _departmentService.GetDoctorsByDepartmentIdAsync(departmentId);

            // ASSERT
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Empty(result.Value);
        }

        [Fact]
        public async Task GetDoctorsByDepartmentIdAsync_RepositoryThrowsException_ReturnsFailureResult()
        {
            // ARRANGE
            var departmentId = Guid.NewGuid();
            _mockRepository.Setup(r => r.GetDoctorsByDepartmentIdAsync(departmentId)).ThrowsAsync(new Exception("Database error"));

            // ACT
            var result = await _departmentService.GetDoctorsByDepartmentIdAsync(departmentId);

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("Error al obtener doctores del departamento", result.ErrorMessage);
        }

        #endregion
    }
}
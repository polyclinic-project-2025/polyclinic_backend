using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Moq;
using AutoMapper;
using PolyclinicApplication.Services.Implementations;
using PolyclinicApplication.DTOs.Request;
using PolyclinicApplication.DTOs.Response;
using PolyclinicDomain.Entities;
using PolyclinicDomain.IRepositories;
using PolyclinicApplication.Common.Results;

namespace PolyclinicApplication.Tests.Services
{
    public class WarehouseManagerServiceTests
    {
        private readonly Mock<IWarehouseManagerRepository> _mockRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly WarehouseManagerService _warehouseManagerService;

        public WarehouseManagerServiceTests()
        {
            _mockRepository = new Mock<IWarehouseManagerRepository>();
            _mockMapper = new Mock<IMapper>();
            _warehouseManagerService = new WarehouseManagerService(_mockRepository.Object, _mockMapper.Object);
        }

        #region CreateAsync Tests

        [Fact]
        public async Task CreateAsync_ValidWarehouseManager_ReturnsSuccessResult()
        {
            // ARRANGE
            var createRequest = new CreateWarehouseManagerRequest
            {
                Identification = "12345678",
                Name = "Jefe de Almacén Juan",
                EmploymentStatus = "Active"
            };

            var warehouseManager = new WarehouseManager(
                Guid.NewGuid(),
                createRequest.Identification,
                createRequest.Name,
                createRequest.EmploymentStatus,
                DateTime.UtcNow
            );

            var response = new WarehouseManagerResponse
            {
                EmployeeId = warehouseManager.EmployeeId,
                Name = warehouseManager.Name,
                Identification = warehouseManager.Identification
            };

            _mockRepository.Setup(r => r.ExistsByIdentificationAsync(createRequest.Identification)).ReturnsAsync(false);
            _mockRepository.Setup(r => r.AddAsync(It.IsAny<WarehouseManager>())).ReturnsAsync(warehouseManager);
            _mockMapper.Setup(m => m.Map<WarehouseManagerResponse>(It.IsAny<WarehouseManager>())).Returns(response);

            // ACT
            var result = await _warehouseManagerService.CreateAsync(createRequest);

            // ASSERT
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(createRequest.Name, result.Value.Name);
            Assert.Equal(createRequest.Identification, result.Value.Identification);
            _mockRepository.Verify(r => r.AddAsync(It.IsAny<WarehouseManager>()), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_DuplicateIdentification_ReturnsFailureResult()
        {
            // ARRANGE
            var createRequest = new CreateWarehouseManagerRequest
            {
                Identification = "12345678",
                Name = "Test Manager",
                EmploymentStatus = "Active"
            };

            _mockRepository.Setup(r => r.ExistsByIdentificationAsync(createRequest.Identification)).ReturnsAsync(true);

            // ACT
            var result = await _warehouseManagerService.CreateAsync(createRequest);

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("Ya existe un empleado con esta identificación", result.ErrorMessage);
            _mockRepository.Verify(r => r.AddAsync(It.IsAny<WarehouseManager>()), Times.Never);
        }

        [Fact]
        public async Task CreateAsync_RepositoryThrowsException_ReturnsFailureResult()
        {
            // ARRANGE
            var createRequest = new CreateWarehouseManagerRequest
            {
                Identification = "12345678",
                Name = "Test Manager",
                EmploymentStatus = "Active"
            };

            _mockRepository.Setup(r => r.ExistsByIdentificationAsync(createRequest.Identification)).ReturnsAsync(false);
            _mockRepository.Setup(r => r.AddAsync(It.IsAny<WarehouseManager>())).ThrowsAsync(new Exception("Database error"));

            // ACT
            var result = await _warehouseManagerService.CreateAsync(createRequest);

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("Error al guardar el jefe de almacén", result.ErrorMessage);
        }

        #endregion

        #region GetWarehouseManagerAsync Tests

        [Fact]
        public async Task GetWarehouseManagerAsync_ManagerExists_ReturnsSuccessWithManager()
        {
            // ARRANGE
            var warehouseManager = new WarehouseManager(
                Guid.NewGuid(),
                "12345678",
                "Jefe de Almacén",
                "Active",
                DateTime.UtcNow
            );

            var response = new WarehouseManagerResponse
            {
                EmployeeId = warehouseManager.EmployeeId,
                Name = warehouseManager.Name
            };

            _mockRepository.Setup(r => r.GetAsync()).ReturnsAsync(warehouseManager);
            _mockMapper.Setup(m => m.Map<WarehouseManagerResponse>(warehouseManager)).Returns(response);

            // ACT
            var result = await _warehouseManagerService.GetWarehouseManagerAsync();

            // ASSERT
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(warehouseManager.Name, result.Value.Name);
            _mockRepository.Verify(r => r.GetAsync(), Times.Once);
        }

        [Fact]
        public async Task GetWarehouseManagerAsync_ManagerNotFound_ReturnsFailureResult()
        {
            // ARRANGE
            _mockRepository.Setup(r => r.GetAsync()).ReturnsAsync((WarehouseManager?)null);

            // ACT
            var result = await _warehouseManagerService.GetWarehouseManagerAsync();

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("Jefe de almacén no encontrado", result.ErrorMessage);
        }

        [Fact]
        public async Task GetWarehouseManagerAsync_RepositoryThrowsException_ReturnsFailureResult()
        {
            // ARRANGE
            _mockRepository.Setup(r => r.GetAsync()).ThrowsAsync(new Exception("Database error"));

            // ACT
            var result = await _warehouseManagerService.GetWarehouseManagerAsync();

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("Error al obtener el jefe de almacén", result.ErrorMessage);
        }

        #endregion

        #region GetAllAsync Tests

        [Fact]
        public async Task GetAllAsync_ManagersExist_ReturnsSuccessWithList()
        {
            // ARRANGE
            var managers = new List<WarehouseManager>
            {
                new WarehouseManager(Guid.NewGuid(), "12345678", "Manager 1", "Active", DateTime.UtcNow),
                new WarehouseManager(Guid.NewGuid(), "87654321", "Manager 2", "Active", DateTime.UtcNow)
            };

            var responses = new List<WarehouseManagerResponse>
            {
                new WarehouseManagerResponse { EmployeeId = managers[0].EmployeeId, Name = "Manager 1" },
                new WarehouseManagerResponse { EmployeeId = managers[1].EmployeeId, Name = "Manager 2" }
            };

            _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(managers);
            _mockMapper.Setup(m => m.Map<IEnumerable<WarehouseManagerResponse>>(managers)).Returns(responses);

            // ACT
            var result = await _warehouseManagerService.GetAllAsync();

            // ASSERT
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(2, result.Value.Count());
            _mockRepository.Verify(r => r.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetAllAsync_NoManagers_ReturnsSuccessWithEmptyList()
        {
            // ARRANGE
            var emptyList = new List<WarehouseManager>();
            _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(emptyList);
            _mockMapper.Setup(m => m.Map<IEnumerable<WarehouseManagerResponse>>(emptyList)).Returns(new List<WarehouseManagerResponse>());

            // ACT
            var result = await _warehouseManagerService.GetAllAsync();

            // ASSERT
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Empty(result.Value);
        }

        #endregion

        #region GetByIdAsync Tests

        [Fact]
        public async Task GetByIdAsync_ValidId_ReturnsSuccessWithManager()
        {
            // ARRANGE
            var managerId = Guid.NewGuid();
            var manager = new WarehouseManager(managerId, "12345678", "Jefe de Almacén", "Active", DateTime.UtcNow);
            var response = new WarehouseManagerResponse { EmployeeId = managerId, Name = "Jefe de Almacén" };

            _mockRepository.Setup(r => r.GetByIdAsync(managerId)).ReturnsAsync(manager);
            _mockMapper.Setup(m => m.Map<WarehouseManagerResponse>(manager)).Returns(response);

            // ACT
            var result = await _warehouseManagerService.GetByIdAsync(managerId);

            // ASSERT
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(managerId, result.Value.EmployeeId);
            Assert.Equal("Jefe de Almacén", result.Value.Name);
            _mockRepository.Verify(r => r.GetByIdAsync(managerId), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_ManagerNotFound_ReturnsFailureResult()
        {
            // ARRANGE
            var managerId = Guid.NewGuid();
            _mockRepository.Setup(r => r.GetByIdAsync(managerId)).ReturnsAsync((WarehouseManager?)null);

            // ACT
            var result = await _warehouseManagerService.GetByIdAsync(managerId);

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("Empleado no encontrado", result.ErrorMessage);
        }

        #endregion

        #region UpdateAsync Tests

        [Fact]
        public async Task UpdateAsync_ValidUpdate_ReturnsSuccessResult()
        {
            // ARRANGE
            var managerId = Guid.NewGuid();
            var existingManager = new WarehouseManager(managerId, "12345678", "Old Name", "Active", DateTime.UtcNow);
            var updateRequest = new UpdateWarehouseManagerRequest
            {
                Name = "New Name",
                Identification = "87654321",
                EmploymentStatus = "Inactive"
            };

            _mockRepository.Setup(r => r.GetByIdAsync(managerId)).ReturnsAsync(existingManager);
            _mockRepository.Setup(r => r.ExistsByIdentificationAsync(updateRequest.Identification)).ReturnsAsync(false);
            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<WarehouseManager>())).Returns(Task.CompletedTask);

            // ACT
            var result = await _warehouseManagerService.UpdateAsync(managerId, updateRequest);

            // ASSERT
            Assert.True(result.IsSuccess);
            Assert.True(result.Value);
            _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<WarehouseManager>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ManagerNotFound_ReturnsFailureResult()
        {
            // ARRANGE
            var managerId = Guid.NewGuid();
            var updateRequest = new UpdateWarehouseManagerRequest { Name = "New Name" };
            _mockRepository.Setup(r => r.GetByIdAsync(managerId)).ReturnsAsync((WarehouseManager?)null);

            // ACT
            var result = await _warehouseManagerService.UpdateAsync(managerId, updateRequest);

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("Jefe de almacén no encontrado", result.ErrorMessage);
            _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<WarehouseManager>()), Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_DuplicateIdentification_ReturnsFailureResult()
        {
            // ARRANGE
            var managerId = Guid.NewGuid();
            var existingManager = new WarehouseManager(managerId, "12345678", "Manager Name", "Active", DateTime.UtcNow);
            var updateRequest = new UpdateWarehouseManagerRequest { Identification = "87654321" };

            _mockRepository.Setup(r => r.GetByIdAsync(managerId)).ReturnsAsync(existingManager);
            _mockRepository.Setup(r => r.ExistsByIdentificationAsync(updateRequest.Identification)).ReturnsAsync(true);

            // ACT
            var result = await _warehouseManagerService.UpdateAsync(managerId, updateRequest);

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("Ya existe un jefe de almacén con esta identificación", result.ErrorMessage);
            _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<WarehouseManager>()), Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_SameIdentification_SkipsValidation()
        {
            // ARRANGE
            var managerId = Guid.NewGuid();
            var existingManager = new WarehouseManager(managerId, "12345678", "Manager Name", "Active", DateTime.UtcNow);
            var updateRequest = new UpdateWarehouseManagerRequest
            {
                Identification = "12345678",
                Name = "Updated Name"
            };

            _mockRepository.Setup(r => r.GetByIdAsync(managerId)).ReturnsAsync(existingManager);
            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<WarehouseManager>())).Returns(Task.CompletedTask);

            // ACT
            var result = await _warehouseManagerService.UpdateAsync(managerId, updateRequest);

            // ASSERT
            Assert.True(result.IsSuccess);
            _mockRepository.Verify(r => r.ExistsByIdentificationAsync(It.IsAny<string>()), Times.Never);
            _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<WarehouseManager>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_RepositoryThrowsException_ReturnsFailureResult()
        {
            // ARRANGE
            var managerId = Guid.NewGuid();
            var existingManager = new WarehouseManager(managerId, "12345678", "Manager Name", "Active", DateTime.UtcNow);
            var updateRequest = new UpdateWarehouseManagerRequest { Name = "New Name" };

            _mockRepository.Setup(r => r.GetByIdAsync(managerId)).ReturnsAsync(existingManager);
            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<WarehouseManager>())).ThrowsAsync(new Exception("Database error"));

            // ACT
            var result = await _warehouseManagerService.UpdateAsync(managerId, updateRequest);

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("Error al actualizar el jefe de almacén", result.ErrorMessage);
        }

        #endregion

        #region DeleteAsync Tests

        [Fact]
        public async Task DeleteAsync_ValidId_ReturnsSuccessResult()
        {
            // ARRANGE
            var managerId = Guid.NewGuid();
            var manager = new WarehouseManager(managerId, "12345678", "Jefe de Almacén", "Active", DateTime.UtcNow);

            _mockRepository.Setup(r => r.GetByIdAsync(managerId)).ReturnsAsync(manager);
            _mockRepository.Setup(r => r.DeleteAsync(manager)).Returns(Task.CompletedTask);

            // ACT
            var result = await _warehouseManagerService.DeleteAsync(managerId);

            // ASSERT
            Assert.True(result.IsSuccess);
            Assert.True(result.Value);
            _mockRepository.Verify(r => r.GetByIdAsync(managerId), Times.Once);
            _mockRepository.Verify(r => r.DeleteAsync(manager), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ManagerNotFound_ReturnsFailureResult()
        {
            // ARRANGE
            var managerId = Guid.NewGuid();
            _mockRepository.Setup(r => r.GetByIdAsync(managerId)).ReturnsAsync((WarehouseManager?)null);

            // ACT
            var result = await _warehouseManagerService.DeleteAsync(managerId);

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("Empleado no encontrado", result.ErrorMessage);
            _mockRepository.Verify(r => r.DeleteAsync(It.IsAny<WarehouseManager>()), Times.Never);
        }

        [Fact]
        public async Task DeleteAsync_RepositoryThrowsException_ReturnsFailureResult()
        {
            // ARRANGE
            var managerId = Guid.NewGuid();
            var manager = new WarehouseManager(managerId, "12345678", "Jefe de Almacén", "Active", DateTime.UtcNow);

            _mockRepository.Setup(r => r.GetByIdAsync(managerId)).ReturnsAsync(manager);
            _mockRepository.Setup(r => r.DeleteAsync(manager)).ThrowsAsync(new Exception("Database error"));

            // ACT
            var result = await _warehouseManagerService.DeleteAsync(managerId);

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("Error al eliminar el empleado", result.ErrorMessage);
        }

        #endregion
    }
}
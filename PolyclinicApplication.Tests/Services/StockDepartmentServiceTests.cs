using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Moq;
using AutoMapper;
using PolyclinicApplication.Services.Implementations;
using PolyclinicApplication.DTOs.Request.StockDepartment;
using PolyclinicApplication.DTOs.Response;
using PolyclinicDomain.Entities;
using PolyclinicDomain.IRepositories;
using PolyclinicApplication.Common.Results;

namespace PolyclinicApplication.Tests.Services
{
    public class StockDepartmentServiceTests
    {
        private readonly Mock<IStockDepartmentRepository> _mockRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly StockDepartmentService _service;

        public StockDepartmentServiceTests()
        {
            _mockRepository = new Mock<IStockDepartmentRepository>();
            _mockMapper = new Mock<IMapper>();
            _service = new StockDepartmentService(
                _mockRepository.Object,
                _mockMapper.Object
            );
        }

        #region CreateAsync Tests

        [Fact]
        public async Task CreateAsync_ValidStockDepartment_ReturnsSuccessResult()
        {
            // ARRANGE
            var createDto = new CreateStockDepartmentDto
            {
                Quantity = 100,
                DepartmentId = Guid.NewGuid(),
                MedicationId = Guid.NewGuid(),
                MinQuantity = 10,
                MaxQuantity = 500
            };

            var stockDepartment = new StockDepartment(
                Guid.NewGuid(),
                createDto.Quantity,
                createDto.DepartmentId,
                createDto.MedicationId,
                createDto.MinQuantity,
                createDto.MaxQuantity
            );

            var stockDepartmentDto = new StockDepartmentDto
            {
                StockDepartmentId = stockDepartment.StockDepartmentId,
                Quantity = stockDepartment.Quantity,
                DepartmentId = stockDepartment.DepartmentId,
                MedicationId = stockDepartment.MedicationId,
                MinQuantity = stockDepartment.MinQuantity,
                MaxQuantity = stockDepartment.MaxQuantity
            };

            _mockRepository.Setup(r => r.AddAsync(It.IsAny<StockDepartment>()))
                .ReturnsAsync(stockDepartment);
            _mockMapper.Setup(m => m.Map<StockDepartmentDto>(It.IsAny<StockDepartment>()))
                .Returns(stockDepartmentDto);

            // ACT
            var result = await _service.CreateAsync(createDto);

            // ASSERT
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(createDto.Quantity, result.Value.Quantity);
            Assert.Equal(createDto.MinQuantity, result.Value.MinQuantity);
            Assert.Equal(createDto.MaxQuantity, result.Value.MaxQuantity);
            _mockRepository.Verify(r => r.AddAsync(It.IsAny<StockDepartment>()), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_RepositoryThrowsException_ReturnsFailureResult()
        {
            // ARRANGE
            var createDto = new CreateStockDepartmentDto
            {
                Quantity = 100,
                DepartmentId = Guid.NewGuid(),
                MedicationId = Guid.NewGuid(),
                MinQuantity = 10,
                MaxQuantity = 500
            };

            _mockRepository.Setup(r => r.AddAsync(It.IsAny<StockDepartment>()))
                .ThrowsAsync(new Exception("Database error"));

            // ACT
            var result = await _service.CreateAsync(createDto);

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("Error", result.ErrorMessage);
        }

        #endregion

        #region GetByIdAsync Tests

        [Fact]
        public async Task GetByIdAsync_ValidId_ReturnsSuccessWithStockDepartment()
        {
            // ARRANGE
            var stockDepartmentId = Guid.NewGuid();
            var stockDepartment = new StockDepartment(
                stockDepartmentId,
                100,
                Guid.NewGuid(),
                Guid.NewGuid(),
                10,
                500
            );

            var stockDepartmentDto = new StockDepartmentDto
            {
                StockDepartmentId = stockDepartmentId,
                Quantity = 100,
                MinQuantity = 10,
                MaxQuantity = 500
            };

            _mockRepository.Setup(r => r.GetByIdAsync(stockDepartmentId))
                .ReturnsAsync(stockDepartment);
            _mockMapper.Setup(m => m.Map<StockDepartmentDto>(stockDepartment))
                .Returns(stockDepartmentDto);

            // ACT
            var result = await _service.GetByIdAsync(stockDepartmentId);

            // ASSERT
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(stockDepartmentId, result.Value.StockDepartmentId);
        }

        [Fact]
        public async Task GetByIdAsync_StockDepartmentNotFound_ReturnsFailureResult()
        {
            // ARRANGE
            var stockDepartmentId = Guid.NewGuid();
            _mockRepository.Setup(r => r.GetByIdAsync(stockDepartmentId))
                .ReturnsAsync((StockDepartment?)null);

            // ACT
            var result = await _service.GetByIdAsync(stockDepartmentId);

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("no fue encontrado", result.ErrorMessage);
        }

        [Fact]
        public async Task GetByIdAsync_RepositoryThrowsException_ReturnsFailureResult()
        {
            // ARRANGE
            var stockDepartmentId = Guid.NewGuid();
            _mockRepository.Setup(r => r.GetByIdAsync(stockDepartmentId))
                .ThrowsAsync(new Exception("Database error"));

            // ACT
            var result = await _service.GetByIdAsync(stockDepartmentId);

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("Error", result.ErrorMessage);
        }

        #endregion

        #region GetAllAsync Tests

        [Fact]
        public async Task GetAllAsync_StockDepartmentsExist_ReturnsSuccessWithList()
        {
            // ARRANGE
            var stockDepartments = new List<StockDepartment>
            {
                new StockDepartment(Guid.NewGuid(), 100, Guid.NewGuid(), Guid.NewGuid(), 10, 500),
                new StockDepartment(Guid.NewGuid(), 200, Guid.NewGuid(), Guid.NewGuid(), 20, 600)
            };

            var stockDepartmentDtos = new List<StockDepartmentDto>
            {
                new StockDepartmentDto { StockDepartmentId = stockDepartments[0].StockDepartmentId, Quantity = 100 },
                new StockDepartmentDto { StockDepartmentId = stockDepartments[1].StockDepartmentId, Quantity = 200 }
            };

            _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(stockDepartments);
            _mockMapper.Setup(m => m.Map<IEnumerable<StockDepartmentDto>>(stockDepartments))
                .Returns(stockDepartmentDtos);

            // ACT
            var result = await _service.GetAllAsync();

            // ASSERT
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(2, result.Value.Count());
        }

        [Fact]
        public async Task GetAllAsync_NoStockDepartments_ReturnsSuccessWithEmptyList()
        {
            // ARRANGE
            var emptyList = new List<StockDepartment>();
            _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(emptyList);
            _mockMapper.Setup(m => m.Map<IEnumerable<StockDepartmentDto>>(emptyList))
                .Returns(new List<StockDepartmentDto>());

            // ACT
            var result = await _service.GetAllAsync();

            // ASSERT
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Empty(result.Value);
        }

        [Fact]
        public async Task GetAllAsync_RepositoryThrowsException_ReturnsFailureResult()
        {
            // ARRANGE
            _mockRepository.Setup(r => r.GetAllAsync())
                .ThrowsAsync(new Exception("Database error"));

            // ACT
            var result = await _service.GetAllAsync();

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("Error", result.ErrorMessage);
        }

        #endregion

        #region UpdateAsync Tests

        [Fact]
        public async Task UpdateAsync_ValidUpdate_ReturnsSuccessResult()
        {
            // ARRANGE
            var stockDepartmentId = Guid.NewGuid();
            var existingStock = new StockDepartment(
                stockDepartmentId,
                100,
                Guid.NewGuid(),
                Guid.NewGuid(),
                10,
                500
            );

            var updateDto = new UpdateStockDepartmentDto
            {
                Quantity = 150,
                MinQuantity = 20,
                MaxQuantity = 600
            };

            _mockRepository.Setup(r => r.GetByIdAsync(stockDepartmentId))
                .ReturnsAsync(existingStock);
            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<StockDepartment>()))
                .Returns(Task.CompletedTask);

            // ACT
            var result = await _service.UpdateAsync(stockDepartmentId, updateDto);

            // ASSERT
            Assert.True(result.IsSuccess);
            Assert.True(result.Value);
            _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<StockDepartment>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_StockDepartmentNotFound_ReturnsFailureResult()
        {
            // ARRANGE
            var stockDepartmentId = Guid.NewGuid();
            var updateDto = new UpdateStockDepartmentDto { Quantity = 150 };

            _mockRepository.Setup(r => r.GetByIdAsync(stockDepartmentId))
                .ReturnsAsync((StockDepartment?)null);

            // ACT
            var result = await _service.UpdateAsync(stockDepartmentId, updateDto);

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("no fue encontrado", result.ErrorMessage);
            _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<StockDepartment>()), Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_UpdateQuantityOnly_ReturnsSuccessResult()
        {
            // ARRANGE
            var stockDepartmentId = Guid.NewGuid();
            var existingStock = new StockDepartment(
                stockDepartmentId,
                100,
                Guid.NewGuid(),
                Guid.NewGuid(),
                10,
                500
            );

            var updateDto = new UpdateStockDepartmentDto { Quantity = 200 };

            _mockRepository.Setup(r => r.GetByIdAsync(stockDepartmentId))
                .ReturnsAsync(existingStock);
            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<StockDepartment>()))
                .Returns(Task.CompletedTask);

            // ACT
            var result = await _service.UpdateAsync(stockDepartmentId, updateDto);

            // ASSERT
            Assert.True(result.IsSuccess);
            _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<StockDepartment>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_UpdateMinQuantityOnly_ReturnsSuccessResult()
        {
            // ARRANGE
            var stockDepartmentId = Guid.NewGuid();
            var existingStock = new StockDepartment(
                stockDepartmentId,
                100,
                Guid.NewGuid(),
                Guid.NewGuid(),
                10,
                500
            );

            var updateDto = new UpdateStockDepartmentDto { MinQuantity = 25 };

            _mockRepository.Setup(r => r.GetByIdAsync(stockDepartmentId))
                .ReturnsAsync(existingStock);
            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<StockDepartment>()))
                .Returns(Task.CompletedTask);

            // ACT
            var result = await _service.UpdateAsync(stockDepartmentId, updateDto);

            // ASSERT
            Assert.True(result.IsSuccess);
            _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<StockDepartment>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_UpdateMaxQuantityOnly_ReturnsSuccessResult()
        {
            // ARRANGE
            var stockDepartmentId = Guid.NewGuid();
            var existingStock = new StockDepartment(
                stockDepartmentId,
                100,
                Guid.NewGuid(),
                Guid.NewGuid(),
                10,
                500
            );

            var updateDto = new UpdateStockDepartmentDto { MaxQuantity = 700 };

            _mockRepository.Setup(r => r.GetByIdAsync(stockDepartmentId))
                .ReturnsAsync(existingStock);
            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<StockDepartment>()))
                .Returns(Task.CompletedTask);

            // ACT
            var result = await _service.UpdateAsync(stockDepartmentId, updateDto);

            // ASSERT
            Assert.True(result.IsSuccess);
            _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<StockDepartment>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_RepositoryThrowsException_ReturnsFailureResult()
        {
            // ARRANGE
            var stockDepartmentId = Guid.NewGuid();
            var existingStock = new StockDepartment(
                stockDepartmentId,
                100,
                Guid.NewGuid(),
                Guid.NewGuid(),
                10,
                500
            );

            var updateDto = new UpdateStockDepartmentDto { Quantity = 150 };

            _mockRepository.Setup(r => r.GetByIdAsync(stockDepartmentId))
                .ReturnsAsync(existingStock);
            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<StockDepartment>()))
                .ThrowsAsync(new Exception("Database error"));

            // ACT
            var result = await _service.UpdateAsync(stockDepartmentId, updateDto);

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("Error", result.ErrorMessage);
        }

        #endregion

        #region DeleteAsync Tests

        [Fact]
        public async Task DeleteAsync_ValidId_ReturnsSuccessResult()
        {
            // ARRANGE
            var stockDepartmentId = Guid.NewGuid();
            var stockDepartment = new StockDepartment(
                stockDepartmentId,
                100,
                Guid.NewGuid(),
                Guid.NewGuid(),
                10,
                500
            );

            _mockRepository.Setup(r => r.GetByIdAsync(stockDepartmentId))
                .ReturnsAsync(stockDepartment);
            _mockRepository.Setup(r => r.DeleteAsync(stockDepartment))
                .Returns(Task.CompletedTask);

            // ACT
            var result = await _service.DeleteAsync(stockDepartmentId);

            // ASSERT
            Assert.True(result.IsSuccess);
            Assert.True(result.Value);
            _mockRepository.Verify(r => r.DeleteAsync(stockDepartment), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_StockDepartmentNotFound_ReturnsFailureResult()
        {
            // ARRANGE
            var stockDepartmentId = Guid.NewGuid();
            _mockRepository.Setup(r => r.GetByIdAsync(stockDepartmentId))
                .ReturnsAsync((StockDepartment?)null);

            // ACT
            var result = await _service.DeleteAsync(stockDepartmentId);

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("no fue encontrado", result.ErrorMessage);
            _mockRepository.Verify(r => r.DeleteAsync(It.IsAny<StockDepartment>()), Times.Never);
        }

        [Fact]
        public async Task DeleteAsync_RepositoryThrowsException_ReturnsFailureResult()
        {
            // ARRANGE
            var stockDepartmentId = Guid.NewGuid();
            var stockDepartment = new StockDepartment(
                stockDepartmentId,
                100,
                Guid.NewGuid(),
                Guid.NewGuid(),
                10,
                500
            );

            _mockRepository.Setup(r => r.GetByIdAsync(stockDepartmentId))
                .ReturnsAsync(stockDepartment);
            _mockRepository.Setup(r => r.DeleteAsync(stockDepartment))
                .ThrowsAsync(new Exception("Database error"));

            // ACT
            var result = await _service.DeleteAsync(stockDepartmentId);

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("Error", result.ErrorMessage);
        }

        #endregion

        #region GetStockByDepartmentIdAsync Tests

        [Fact]
        public async Task GetStockByDepartmentIdAsync_ValidDepartmentId_ReturnsSuccessWithList()
        {
            // ARRANGE
            var departmentId = Guid.NewGuid();
            var stockDepartments = new List<StockDepartment>
            {
                new StockDepartment(Guid.NewGuid(), 100, departmentId, Guid.NewGuid(), 10, 500),
                new StockDepartment(Guid.NewGuid(), 200, departmentId, Guid.NewGuid(), 20, 600)
            };

            var stockDepartmentDtos = new List<StockDepartmentDto>
            {
                new StockDepartmentDto { StockDepartmentId = stockDepartments[0].StockDepartmentId, Quantity = 100 },
                new StockDepartmentDto { StockDepartmentId = stockDepartments[1].StockDepartmentId, Quantity = 200 }
            };

            _mockRepository.Setup(r => r.GetStockByDepartmentAsync(departmentId))
                .ReturnsAsync(stockDepartments);
            _mockMapper.Setup(m => m.Map<IEnumerable<StockDepartmentDto>>(stockDepartments))
                .Returns(stockDepartmentDtos);

            // ACT
            var result = await _service.GetStockByDepartmentIdAsync(departmentId);

            // ASSERT
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(2, result.Value.Count());
        }

        [Fact]
        public async Task GetStockByDepartmentIdAsync_NoStockForDepartment_ReturnsSuccessWithEmptyList()
        {
            // ARRANGE
            var departmentId = Guid.NewGuid();
            var emptyList = new List<StockDepartment>();

            _mockRepository.Setup(r => r.GetStockByDepartmentAsync(departmentId))
                .ReturnsAsync(emptyList);
            _mockMapper.Setup(m => m.Map<IEnumerable<StockDepartmentDto>>(emptyList))
                .Returns(new List<StockDepartmentDto>());

            // ACT
            var result = await _service.GetStockByDepartmentIdAsync(departmentId);

            // ASSERT
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Empty(result.Value);
        }

        #endregion

        #region GetLowStockByDepartmentIdAsync Tests

        [Fact]
        public async Task GetLowStockByDepartmentIdAsync_ValidDepartmentId_ReturnsSuccessWithLowStock()
        {
            // ARRANGE
            var departmentId = Guid.NewGuid();
            var lowStockDepartments = new List<StockDepartment>
            {
                new StockDepartment(Guid.NewGuid(), 5, departmentId, Guid.NewGuid(), 10, 500),
                new StockDepartment(Guid.NewGuid(), 8, departmentId, Guid.NewGuid(), 20, 600)
            };

            var stockDepartmentDtos = new List<StockDepartmentDto>
            {
                new StockDepartmentDto { StockDepartmentId = lowStockDepartments[0].StockDepartmentId, Quantity = 5 },
                new StockDepartmentDto { StockDepartmentId = lowStockDepartments[1].StockDepartmentId, Quantity = 8 }
            };

            _mockRepository.Setup(r => r.GetBelowMinQuantityAsync(departmentId))
                .ReturnsAsync(lowStockDepartments);
            _mockMapper.Setup(m => m.Map<IEnumerable<StockDepartmentDto>>(lowStockDepartments))
                .Returns(stockDepartmentDtos);

            // ACT
            var result = await _service.GetLowStockByDepartmentIdAsync(departmentId);

            // ASSERT
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(2, result.Value.Count());
        }

        [Fact]
        public async Task GetLowStockByDepartmentIdAsync_NoLowStock_ReturnsSuccessWithEmptyList()
        {
            // ARRANGE
            var departmentId = Guid.NewGuid();
            var emptyList = new List<StockDepartment>();

            _mockRepository.Setup(r => r.GetBelowMinQuantityAsync(departmentId))
                .ReturnsAsync(emptyList);
            _mockMapper.Setup(m => m.Map<IEnumerable<StockDepartmentDto>>(emptyList))
                .Returns(new List<StockDepartmentDto>());

            // ACT
            var result = await _service.GetLowStockByDepartmentIdAsync(departmentId);

            // ASSERT
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Empty(result.Value);
        }

        [Fact]
        public async Task GetLowStockByDepartmentIdAsync_RepositoryThrowsException_ReturnsFailureResult()
        {
            // ARRANGE
            var departmentId = Guid.NewGuid();
            _mockRepository.Setup(r => r.GetBelowMinQuantityAsync(departmentId))
                .ThrowsAsync(new Exception("Database error"));

            // ACT
            var result = await _service.GetLowStockByDepartmentIdAsync(departmentId);

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("Error", result.ErrorMessage);
        }

        #endregion

        #region GetOverStockByDepartmentIdAsync Tests

        [Fact]
        public async Task GetOverStockByDepartmentIdAsync_ValidDepartmentId_ReturnsSuccessWithOverStock()
        {
            // ARRANGE
            var departmentId = Guid.NewGuid();
            var overStockDepartments = new List<StockDepartment>
            {
                new StockDepartment(Guid.NewGuid(), 550, departmentId, Guid.NewGuid(), 10, 500),
                new StockDepartment(Guid.NewGuid(), 650, departmentId, Guid.NewGuid(), 20, 600)
            };

            var stockDepartmentDtos = new List<StockDepartmentDto>
            {
                new StockDepartmentDto { StockDepartmentId = overStockDepartments[0].StockDepartmentId, Quantity = 550 },
                new StockDepartmentDto { StockDepartmentId = overStockDepartments[1].StockDepartmentId, Quantity = 650 }
            };

            _mockRepository.Setup(r => r.GetAboveMaxQuantityAsync(departmentId))
                .ReturnsAsync(overStockDepartments);
            _mockMapper.Setup(m => m.Map<IEnumerable<StockDepartmentDto>>(overStockDepartments))
                .Returns(stockDepartmentDtos);

            // ACT
            var result = await _service.GetOverStockByDepartmentIdAsync(departmentId);

            // ASSERT
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(2, result.Value.Count());
        }

        [Fact]
        public async Task GetOverStockByDepartmentIdAsync_NoOverStock_ReturnsSuccessWithEmptyList()
        {
            // ARRANGE
            var departmentId = Guid.NewGuid();
            var emptyList = new List<StockDepartment>();

            _mockRepository.Setup(r => r.GetAboveMaxQuantityAsync(departmentId))
                .ReturnsAsync(emptyList);
            _mockMapper.Setup(m => m.Map<IEnumerable<StockDepartmentDto>>(emptyList))
                .Returns(new List<StockDepartmentDto>());

            // ACT
            var result = await _service.GetOverStockByDepartmentIdAsync(departmentId);

            // ASSERT
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Empty(result.Value);
        }

        [Fact]
        public async Task GetOverStockByDepartmentIdAsync_RepositoryThrowsException_ReturnsFailureResult()
        {
            // ARRANGE
            var departmentId = Guid.NewGuid();
            _mockRepository.Setup(r => r.GetAboveMaxQuantityAsync(departmentId))
                .ThrowsAsync(new Exception("Database error"));

            // ACT
            var result = await _service.GetOverStockByDepartmentIdAsync(departmentId);

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("Error", result.ErrorMessage);
        }

        #endregion
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Moq;
using AutoMapper;
using PolyclinicApplication.Services.Implementations;
using PolyclinicApplication.DTOs.Request.MedicationDerivation;
using PolyclinicApplication.DTOs.Response;
using PolyclinicDomain.Entities;
using PolyclinicDomain.IRepositories;
using PolyclinicApplication.Common.Results;

namespace PolyclinicApplication.Tests.Services
{
    public class MedicationDerivationServiceTests
    {
        private readonly Mock<IMedicationDerivationRepository> _mockRepository;
        private readonly Mock<IConsultationDerivationRepository> _mockConsultationRepo;
        private readonly Mock<IStockDepartmentRepository> _mockStockRepo;
        private readonly Mock<IMapper> _mockMapper;
        private readonly MedicationDerivationService _service;

        public MedicationDerivationServiceTests()
        {
            _mockRepository = new Mock<IMedicationDerivationRepository>();
            _mockConsultationRepo = new Mock<IConsultationDerivationRepository>();
            _mockStockRepo = new Mock<IStockDepartmentRepository>();
            _mockMapper = new Mock<IMapper>();
            _service = new MedicationDerivationService(
                _mockRepository.Object,
                _mockConsultationRepo.Object,
                _mockStockRepo.Object,
                _mockMapper.Object
            );
        }

        #region CreateAsync Tests

        [Fact]
        public async Task CreateAsync_ValidRequest_DecreasesStockAndReturnsSuccess()
        {
            // ARRANGE
            var departmentId = Guid.NewGuid();
            var createDto = new CreateMedicationDerivationDto
            {
                Quantity = 5,
                ConsultationDerivationId = Guid.NewGuid(),
                MedicationId = Guid.NewGuid()
            };

            var department = new Department(departmentId, "Cardiología");
            var departmentHead = new DepartmentHead(Guid.NewGuid(), Guid.NewGuid(), departmentId, DateTime.Now);
            
            var consultationDerivation = new ConsultationDerivation(
                createDto.ConsultationDerivationId,
                "Diagnosis",
                Guid.NewGuid(),
                DateTime.Now,
                Guid.NewGuid(),
                departmentHead.DepartmentHeadId
            );

            var stock = new StockDepartment(Guid.NewGuid(), createDto.MedicationId, departmentId, 10);

            var medicationDerivation = new MedicationDerivation(
                Guid.NewGuid(),
                createDto.Quantity,
                createDto.ConsultationDerivationId,
                createDto.MedicationId
            );

            var medicationDerivationDto = new MedicationDerivationDto
            {
                MedicationDerivationId = medicationDerivation.MedicationDerivationId,
                Quantity = createDto.Quantity
            };

            _mockConsultationRepo.Setup(r => r.GetWithDepartmentAsync(createDto.ConsultationDerivationId))
                .ReturnsAsync(consultationDerivation);
            _mockStockRepo.Setup(r => r.GetByDepartmentAndMedicationAsync(departmentId, createDto.MedicationId))
                .ReturnsAsync(stock);
            _mockStockRepo.Setup(r => r.UpdateAsync(It.IsAny<StockDepartment>())).Returns(Task.CompletedTask);
            _mockRepository.Setup(r => r.AddAsync(It.IsAny<MedicationDerivation>())).ReturnsAsync(medicationDerivation);
            _mockMapper.Setup(m => m.Map<MedicationDerivationDto>(It.IsAny<MedicationDerivation>()))
                .Returns(medicationDerivationDto);

            // ACT
            var result = await _service.CreateAsync(createDto);

            // ASSERT
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(createDto.Quantity, result.Value.Quantity);
            _mockStockRepo.Verify(r => r.UpdateAsync(It.Is<StockDepartment>(s => s.Quantity == 5)), Times.Once);
            _mockRepository.Verify(r => r.AddAsync(It.IsAny<MedicationDerivation>()), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_ConsultationDerivationNotFound_ReturnsFailureResult()
        {
            // ARRANGE
            var createDto = new CreateMedicationDerivationDto
            {
                Quantity = 5,
                ConsultationDerivationId = Guid.NewGuid(),
                MedicationId = Guid.NewGuid()
            };

            _mockConsultationRepo.Setup(r => r.GetWithDepartmentAsync(createDto.ConsultationDerivationId))
                .ReturnsAsync((ConsultationDerivation?)null);

            // ACT
            var result = await _service.CreateAsync(createDto);

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("La consulta de derivación no fue encontrada", result.ErrorMessage);
            _mockRepository.Verify(r => r.AddAsync(It.IsAny<MedicationDerivation>()), Times.Never);
        }

        [Fact]
        public async Task CreateAsync_DepartmentNotFound_ReturnsFailureResult()
        {
            // ARRANGE
            var createDto = new CreateMedicationDerivationDto
            {
                Quantity = 5,
                ConsultationDerivationId = Guid.NewGuid(),
                MedicationId = Guid.NewGuid()
            };

            var consultationDerivation = new ConsultationDerivation(
                createDto.ConsultationDerivationId,
                "Diagnosis",
                Guid.NewGuid(),
                DateTime.Now,
                Guid.NewGuid(),
                Guid.NewGuid()
            );

            _mockConsultationRepo.Setup(r => r.GetWithDepartmentAsync(createDto.ConsultationDerivationId))
                .ReturnsAsync(consultationDerivation);

            // ACT
            var result = await _service.CreateAsync(createDto);

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("No se pudo obtener el departamento de la consulta", result.ErrorMessage);
            _mockRepository.Verify(r => r.AddAsync(It.IsAny<MedicationDerivation>()), Times.Never);
        }

        [Fact]
        public async Task CreateAsync_StockNotFound_ReturnsFailureResult()
        {
            // ARRANGE
            var departmentId = Guid.NewGuid();
            var createDto = new CreateMedicationDerivationDto
            {
                Quantity = 5,
                ConsultationDerivationId = Guid.NewGuid(),
                MedicationId = Guid.NewGuid()
            };

            var department = new Department(departmentId, "Cardiología");
            var departmentHead = new DepartmentHead(Guid.NewGuid(), Guid.NewGuid(), departmentId, DateTime.Now);
            
            var consultationDerivation = new ConsultationDerivation(
                createDto.ConsultationDerivationId,
                "Diagnosis",
                Guid.NewGuid(),
                DateTime.Now,
                Guid.NewGuid(),
                departmentHead.DepartmentHeadId
            );

            _mockConsultationRepo.Setup(r => r.GetWithDepartmentAsync(createDto.ConsultationDerivationId))
                .ReturnsAsync(consultationDerivation);
            _mockStockRepo.Setup(r => r.GetByDepartmentAndMedicationAsync(departmentId, createDto.MedicationId))
                .ReturnsAsync((StockDepartment?)null);

            // ACT
            var result = await _service.CreateAsync(createDto);

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("No existe stock del medicamento en el departamento especificado", result.ErrorMessage);
            _mockRepository.Verify(r => r.AddAsync(It.IsAny<MedicationDerivation>()), Times.Never);
        }

        [Fact]
        public async Task CreateAsync_InsufficientStock_ReturnsFailureResult()
        {
            // ARRANGE
            var departmentId = Guid.NewGuid();
            var createDto = new CreateMedicationDerivationDto
            {
                Quantity = 15,
                ConsultationDerivationId = Guid.NewGuid(),
                MedicationId = Guid.NewGuid()
            };

            var department = new Department(departmentId, "Cardiología");
            var departmentHead = new DepartmentHead(Guid.NewGuid(), Guid.NewGuid(), departmentId, DateTime.Now);
            
            var consultationDerivation = new ConsultationDerivation(
                createDto.ConsultationDerivationId,
                "Diagnosis",
                Guid.NewGuid(),
                DateTime.Now,
                Guid.NewGuid(),
                departmentHead.DepartmentHeadId
            );

            var stock = new StockDepartment(Guid.NewGuid(), createDto.MedicationId, departmentId, 10);

            _mockConsultationRepo.Setup(r => r.GetWithDepartmentAsync(createDto.ConsultationDerivationId))
                .ReturnsAsync(consultationDerivation);
            _mockStockRepo.Setup(r => r.GetByDepartmentAndMedicationAsync(departmentId, createDto.MedicationId))
                .ReturnsAsync(stock);

            // ACT
            var result = await _service.CreateAsync(createDto);

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("Stock insuficiente", result.ErrorMessage);
            Assert.Contains("Disponible: 10", result.ErrorMessage);
            Assert.Contains("Solicitado: 15", result.ErrorMessage);
            _mockRepository.Verify(r => r.AddAsync(It.IsAny<MedicationDerivation>()), Times.Never);
        }

        [Fact]
        public async Task CreateAsync_RepositoryThrowsException_ReturnsFailureResult()
        {
            // ARRANGE
            var departmentId = Guid.NewGuid();
            var createDto = new CreateMedicationDerivationDto
            {
                Quantity = 5,
                ConsultationDerivationId = Guid.NewGuid(),
                MedicationId = Guid.NewGuid()
            };

            var department = new Department(departmentId, "Cardiología");
            var departmentHead = new DepartmentHead(Guid.NewGuid(), Guid.NewGuid(), departmentId, DateTime.Now);
            
            var consultationDerivation = new ConsultationDerivation(
                createDto.ConsultationDerivationId,
                "Diagnosis",
                Guid.NewGuid(),
                DateTime.Now,
                Guid.NewGuid(),
                departmentHead.DepartmentHeadId
            );

            var stock = new StockDepartment(Guid.NewGuid(), createDto.MedicationId, departmentId, 10);

            _mockConsultationRepo.Setup(r => r.GetWithDepartmentAsync(createDto.ConsultationDerivationId))
                .ReturnsAsync(consultationDerivation);
            _mockStockRepo.Setup(r => r.GetByDepartmentAndMedicationAsync(departmentId, createDto.MedicationId))
                .ReturnsAsync(stock);
            _mockRepository.Setup(r => r.AddAsync(It.IsAny<MedicationDerivation>()))
                .ThrowsAsync(new Exception("Database error"));

            // ACT
            var result = await _service.CreateAsync(createDto);

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("Error al guardar la derivación", result.ErrorMessage);
        }

        #endregion

        #region GetByIdAsync Tests

        [Fact]
        public async Task GetByIdAsync_ValidId_ReturnsSuccessWithMedicationDerivation()
        {
            // ARRANGE
            var medicationDerivationId = Guid.NewGuid();
            var medicationDerivation = new MedicationDerivation(
                medicationDerivationId,
                5,
                Guid.NewGuid(),
                Guid.NewGuid()
            );

            var medicationDerivationDto = new MedicationDerivationDto
            {
                MedicationDerivationId = medicationDerivationId,
                Quantity = 5
            };

            _mockRepository.Setup(r => r.GetByIdAsync(medicationDerivationId)).ReturnsAsync(medicationDerivation);
            _mockMapper.Setup(m => m.Map<MedicationDerivationDto>(medicationDerivation)).Returns(medicationDerivationDto);

            // ACT
            var result = await _service.GetByIdAsync(medicationDerivationId);

            // ASSERT
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(medicationDerivationId, result.Value.MedicationDerivationId);
            _mockRepository.Verify(r => r.GetByIdAsync(medicationDerivationId), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_NotFound_ReturnsFailureResult()
        {
            // ARRANGE
            var medicationDerivationId = Guid.NewGuid();
            _mockRepository.Setup(r => r.GetByIdAsync(medicationDerivationId)).ReturnsAsync((MedicationDerivation?)null);

            // ACT
            var result = await _service.GetByIdAsync(medicationDerivationId);

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("La derivación de medicamento no fue encontrada", result.ErrorMessage);
        }

        [Fact]
        public async Task GetByIdAsync_RepositoryThrowsException_ReturnsFailureResult()
        {
            // ARRANGE
            var medicationDerivationId = Guid.NewGuid();
            _mockRepository.Setup(r => r.GetByIdAsync(medicationDerivationId)).ThrowsAsync(new Exception("Database error"));

            // ACT
            var result = await _service.GetByIdAsync(medicationDerivationId);

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("Error al obtener la derivación", result.ErrorMessage);
        }

        #endregion

        #region GetAllAsync Tests

        [Fact]
        public async Task GetAllAsync_MedicationDerivationsExist_ReturnsSuccessWithList()
        {
            // ARRANGE
            var medicationDerivations = new List<MedicationDerivation>
            {
                new MedicationDerivation(Guid.NewGuid(), 5, Guid.NewGuid(), Guid.NewGuid()),
                new MedicationDerivation(Guid.NewGuid(), 10, Guid.NewGuid(), Guid.NewGuid())
            };

            var medicationDerivationDtos = new List<MedicationDerivationDto>
            {
                new MedicationDerivationDto { MedicationDerivationId = medicationDerivations[0].MedicationDerivationId },
                new MedicationDerivationDto { MedicationDerivationId = medicationDerivations[1].MedicationDerivationId }
            };

            _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(medicationDerivations);
            _mockMapper.Setup(m => m.Map<IEnumerable<MedicationDerivationDto>>(medicationDerivations))
                .Returns(medicationDerivationDtos);

            // ACT
            var result = await _service.GetAllAsync();

            // ASSERT
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(2, result.Value.Count());
            _mockRepository.Verify(r => r.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetAllAsync_NoMedicationDerivations_ReturnsSuccessWithEmptyList()
        {
            // ARRANGE
            var emptyList = new List<MedicationDerivation>();
            _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(emptyList);
            _mockMapper.Setup(m => m.Map<IEnumerable<MedicationDerivationDto>>(emptyList))
                .Returns(new List<MedicationDerivationDto>());

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
            _mockRepository.Setup(r => r.GetAllAsync()).ThrowsAsync(new Exception("Database error"));

            // ACT
            var result = await _service.GetAllAsync();

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("Error al obtener las derivaciones", result.ErrorMessage);
        }

        #endregion

        #region UpdateAsync Tests

        [Fact]
        public async Task UpdateAsync_IncreaseQuantity_DecreasesStockAndReturnsSuccess()
        {
            // ARRANGE
            var medicationDerivationId = Guid.NewGuid();
            var departmentId = Guid.NewGuid();
            var medicationId = Guid.NewGuid();
            var consultationDerivationId = Guid.NewGuid();

            var existingMedicationDerivation = new MedicationDerivation(
                medicationDerivationId,
                5,
                consultationDerivationId,
                medicationId
            );

            var updateDto = new UpdateMedicationDerivationDto
            {
                Quantity = 10
            };

            var department = new Department(departmentId, "Cardiología");
            var departmentHead = new DepartmentHead(Guid.NewGuid(), Guid.NewGuid(), departmentId, DateTime.Now);
            
            var consultationDerivation = new ConsultationDerivation(
                consultationDerivationId,
                "Diagnosis",
                Guid.NewGuid(),
                DateTime.Now,
                Guid.NewGuid(),
                departmentHead.DepartmentHeadId
            );

            var stock = new StockDepartment(Guid.NewGuid(), medicationId, departmentId, 20);

            _mockRepository.Setup(r => r.GetByIdAsync(medicationDerivationId)).ReturnsAsync(existingMedicationDerivation);
            _mockConsultationRepo.Setup(r => r.GetWithDepartmentAsync(consultationDerivationId))
                .ReturnsAsync(consultationDerivation);
            _mockStockRepo.Setup(r => r.GetByDepartmentAndMedicationAsync(departmentId, medicationId)).ReturnsAsync(stock);
            _mockStockRepo.Setup(r => r.UpdateAsync(It.IsAny<StockDepartment>())).Returns(Task.CompletedTask);
            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<MedicationDerivation>())).Returns(Task.CompletedTask);

            // ACT
            var result = await _service.UpdateAsync(medicationDerivationId, updateDto);

            // ASSERT
            Assert.True(result.IsSuccess);
            Assert.True(result.Value);
            _mockStockRepo.Verify(r => r.UpdateAsync(It.Is<StockDepartment>(s => s.Quantity == 15)), Times.Once);
            _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<MedicationDerivation>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_DecreaseQuantity_IncreasesStockAndReturnsSuccess()
        {
            // ARRANGE
            var medicationDerivationId = Guid.NewGuid();
            var departmentId = Guid.NewGuid();
            var medicationId = Guid.NewGuid();
            var consultationDerivationId = Guid.NewGuid();

            var existingMedicationDerivation = new MedicationDerivation(
                medicationDerivationId,
                10,
                consultationDerivationId,
                medicationId
            );

            var updateDto = new UpdateMedicationDerivationDto
            {
                Quantity = 5
            };

            var department = new Department(departmentId, "Cardiología");
            var departmentHead = new DepartmentHead(Guid.NewGuid(), Guid.NewGuid(), departmentId, DateTime.Now);
            
            var consultationDerivation = new ConsultationDerivation(
                consultationDerivationId,
                "Diagnosis",
                Guid.NewGuid(),
                DateTime.Now,
                Guid.NewGuid(),
                departmentHead.DepartmentHeadId
            );

            var stock = new StockDepartment(Guid.NewGuid(), medicationId, departmentId, 10);

            _mockRepository.Setup(r => r.GetByIdAsync(medicationDerivationId)).ReturnsAsync(existingMedicationDerivation);
            _mockConsultationRepo.Setup(r => r.GetWithDepartmentAsync(consultationDerivationId))
                .ReturnsAsync(consultationDerivation);
            _mockStockRepo.Setup(r => r.GetByDepartmentAndMedicationAsync(departmentId, medicationId)).ReturnsAsync(stock);
            _mockStockRepo.Setup(r => r.UpdateAsync(It.IsAny<StockDepartment>())).Returns(Task.CompletedTask);
            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<MedicationDerivation>())).Returns(Task.CompletedTask);

            // ACT
            var result = await _service.UpdateAsync(medicationDerivationId, updateDto);

            // ASSERT
            Assert.True(result.IsSuccess);
            Assert.True(result.Value);
            _mockStockRepo.Verify(r => r.UpdateAsync(It.Is<StockDepartment>(s => s.Quantity == 15)), Times.Once);
            _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<MedicationDerivation>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_MedicationDerivationNotFound_ReturnsFailureResult()
        {
            // ARRANGE
            var medicationDerivationId = Guid.NewGuid();
            var updateDto = new UpdateMedicationDerivationDto { Quantity = 10 };

            _mockRepository.Setup(r => r.GetByIdAsync(medicationDerivationId)).ReturnsAsync((MedicationDerivation?)null);

            // ACT
            var result = await _service.UpdateAsync(medicationDerivationId, updateDto);

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("La derivación de medicamento no fue encontrada", result.ErrorMessage);
            _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<MedicationDerivation>()), Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_InsufficientStockForIncrease_ReturnsFailureResult()
        {
            // ARRANGE
            var medicationDerivationId = Guid.NewGuid();
            var departmentId = Guid.NewGuid();
            var medicationId = Guid.NewGuid();
            var consultationDerivationId = Guid.NewGuid();

            var existingMedicationDerivation = new MedicationDerivation(
                medicationDerivationId,
                5,
                consultationDerivationId,
                medicationId
            );

            var updateDto = new UpdateMedicationDerivationDto
            {
                Quantity = 20
            };

            var department = new Department(departmentId, "Cardiología");
            var departmentHead = new DepartmentHead(Guid.NewGuid(), Guid.NewGuid(), departmentId, DateTime.Now);
            
            var consultationDerivation = new ConsultationDerivation(
                consultationDerivationId,
                "Diagnosis",
                Guid.NewGuid(),
                DateTime.Now,
                Guid.NewGuid(),
                departmentHead.DepartmentHeadId
            );

            var stock = new StockDepartment(Guid.NewGuid(), medicationId, departmentId, 10);

            _mockRepository.Setup(r => r.GetByIdAsync(medicationDerivationId)).ReturnsAsync(existingMedicationDerivation);
            _mockConsultationRepo.Setup(r => r.GetWithDepartmentAsync(consultationDerivationId))
                .ReturnsAsync(consultationDerivation);
            _mockStockRepo.Setup(r => r.GetByDepartmentAndMedicationAsync(departmentId, medicationId)).ReturnsAsync(stock);

            // ACT
            var result = await _service.UpdateAsync(medicationDerivationId, updateDto);

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("Stock insuficiente para aumentar la cantidad", result.ErrorMessage);
            _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<MedicationDerivation>()), Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_RepositoryThrowsException_ReturnsFailureResult()
        {
            // ARRANGE
            var medicationDerivationId = Guid.NewGuid();
            var existingMedicationDerivation = new MedicationDerivation(
                medicationDerivationId,
                5,
                Guid.NewGuid(),
                Guid.NewGuid()
            );

            var updateDto = new UpdateMedicationDerivationDto
            {
                ConsultationDerivationId = Guid.NewGuid()
            };

            _mockRepository.Setup(r => r.GetByIdAsync(medicationDerivationId)).ReturnsAsync(existingMedicationDerivation);
            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<MedicationDerivation>()))
                .ThrowsAsync(new Exception("Database error"));

            // ACT
            var result = await _service.UpdateAsync(medicationDerivationId, updateDto);

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("Error al actualizar la derivación", result.ErrorMessage);
        }

        #endregion

        #region DeleteAsync Tests

        [Fact]
        public async Task DeleteAsync_ValidId_ReturnsStockAndReturnsSuccess()
        {
            // ARRANGE
            var medicationDerivationId = Guid.NewGuid();
            var departmentId = Guid.NewGuid();
            var medicationId = Guid.NewGuid();
            var consultationDerivationId = Guid.NewGuid();

            var medicationDerivation = new MedicationDerivation(
                medicationDerivationId,
                5,
                consultationDerivationId,
                medicationId
            );

            var department = new Department(departmentId, "Cardiología");
            var departmentHead = new DepartmentHead(Guid.NewGuid(), Guid.NewGuid(), departmentId, DateTime.Now);
            
            var consultationDerivation = new ConsultationDerivation(
                consultationDerivationId,
                "Diagnosis",
                Guid.NewGuid(),
                DateTime.Now,
                Guid.NewGuid(),
                departmentHead.DepartmentHeadId
            );

            var stock = new StockDepartment(Guid.NewGuid(), medicationId, departmentId, 10);

            _mockRepository.Setup(r => r.GetByIdAsync(medicationDerivationId)).ReturnsAsync(medicationDerivation);
            _mockConsultationRepo.Setup(r => r.GetWithDepartmentAsync(consultationDerivationId))
                .ReturnsAsync(consultationDerivation);
            _mockStockRepo.Setup(r => r.GetByDepartmentAndMedicationAsync(departmentId, medicationId)).ReturnsAsync(stock);
            _mockStockRepo.Setup(r => r.UpdateAsync(It.IsAny<StockDepartment>())).Returns(Task.CompletedTask);
            _mockRepository.Setup(r => r.DeleteAsync(medicationDerivation)).Returns(Task.CompletedTask);

            // ACT
            var result = await _service.DeleteAsync(medicationDerivationId);

            // ASSERT
            Assert.True(result.IsSuccess);
            Assert.True(result.Value);
            _mockStockRepo.Verify(r => r.UpdateAsync(It.Is<StockDepartment>(s => s.Quantity == 15)), Times.Once);
            _mockRepository.Verify(r => r.DeleteAsync(medicationDerivation), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_MedicationDerivationNotFound_ReturnsFailureResult()
        {
            // ARRANGE
            var medicationDerivationId = Guid.NewGuid();
            _mockRepository.Setup(r => r.GetByIdAsync(medicationDerivationId)).ReturnsAsync((MedicationDerivation?)null);

            // ACT
            var result = await _service.DeleteAsync(medicationDerivationId);

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("La derivación de medicamento no fue encontrada", result.ErrorMessage);
            _mockRepository.Verify(r => r.DeleteAsync(It.IsAny<MedicationDerivation>()), Times.Never);
        }

        [Fact]
        public async Task DeleteAsync_StockNotFound_StillDeletesSuccessfully()
        {
            // ARRANGE
            var medicationDerivationId = Guid.NewGuid();
            var departmentId = Guid.NewGuid();
            var medicationId = Guid.NewGuid();
            var consultationDerivationId = Guid.NewGuid();

            var medicationDerivation = new MedicationDerivation(
                medicationDerivationId,
                5,
                consultationDerivationId,
                medicationId
            );

            var department = new Department(departmentId, "Cardiología");
            var departmentHead = new DepartmentHead(Guid.NewGuid(), Guid.NewGuid(), departmentId, DateTime.Now);
            
            var consultationDerivation = new ConsultationDerivation(
                consultationDerivationId,
                "Diagnosis",
                Guid.NewGuid(),
                DateTime.Now,
                Guid.NewGuid(),
                departmentHead.DepartmentHeadId
            );

            _mockRepository.Setup(r => r.GetByIdAsync(medicationDerivationId))
                .ReturnsAsync(medicationDerivation);
            _mockConsultationRepo.Setup(r => r.GetWithDepartmentAsync(consultationDerivationId))
                .ReturnsAsync(consultationDerivation);
            _mockStockRepo.Setup(r => r.GetByDepartmentAndMedicationAsync(departmentId, medicationId))
                .ReturnsAsync((StockDepartment?)null);
            _mockRepository.Setup(r => r.DeleteAsync(medicationDerivation))
                .Returns(Task.CompletedTask);

            // ACT
            var result = await _service.DeleteAsync(medicationDerivationId);

            // ASSERT
            Assert.True(result.IsSuccess);
            Assert.True(result.Value);
            _mockStockRepo.Verify(r => r.UpdateAsync(It.IsAny<StockDepartment>()), Times.Never);
            _mockRepository.Verify(r => r.DeleteAsync(medicationDerivation), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_RepositoryThrowsException_ReturnsFailureResult()
        {
            // ARRANGE
            var medicationDerivationId = Guid.NewGuid();
            var medicationDerivation = new MedicationDerivation(
                medicationDerivationId,
                5,
                Guid.NewGuid(),
                Guid.NewGuid()
            );

            _mockRepository.Setup(r => r.GetByIdAsync(medicationDerivationId))
                .ReturnsAsync(medicationDerivation);
            _mockRepository.Setup(r => r.DeleteAsync(medicationDerivation))
                .ThrowsAsync(new Exception("Database error"));

            // ACT
            var result = await _service.DeleteAsync(medicationDerivationId);

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("Error al eliminar la derivación", result.ErrorMessage);
        }

        #endregion

        
    }
}    
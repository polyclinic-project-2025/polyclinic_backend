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
    /// <summary>
    /// Tests para MedicationDerivationService
    /// Cobertura: CRUD + Lógica de inventario (stock)
    /// Total de tests: 25
    /// </summary>
    public class MedicationDerivationServiceTests
    {
        #region Setup & Dependencies

        private readonly Mock<IMedicationDerivationRepository> _mockRepository;
        private readonly Mock<IConsultationDerivationRepository> _mockConsultationDerivationRepository;
        private readonly Mock<IStockDepartmentRepository> _mockStockDepartmentRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly MedicationDerivationService _service;

        public MedicationDerivationServiceTests()
        {
            _mockRepository = new Mock<IMedicationDerivationRepository>();
            _mockConsultationDerivationRepository = new Mock<IConsultationDerivationRepository>();
            _mockStockDepartmentRepository = new Mock<IStockDepartmentRepository>();
            _mockMapper = new Mock<IMapper>();

            _service = new MedicationDerivationService(
                _mockRepository.Object,
                _mockConsultationDerivationRepository.Object,
                _mockStockDepartmentRepository.Object,
                _mockMapper.Object
            );
        }

        #endregion

        #region CreateAsync Tests

        [Fact]
        public async Task CreateAsync_ValidRequest_CreatesAndDecreasesStock()
        {
            // ARRANGE
            var consultationDerivationId = Guid.NewGuid();
            var medicationId = Guid.NewGuid();
            var departmentId = Guid.NewGuid();
            var stockId = Guid.NewGuid();

            var createDto = new CreateMedicationDerivationDto
            {
                Quantity = 10,
                ConsultationDerivationId = consultationDerivationId,
                MedicationId = medicationId
            };

            var department = new Department(departmentId, "Cardiología");
            var departmentHead = new DepartmentHead(Guid.NewGuid(), Guid.NewGuid(), departmentId, DateTime.UtcNow);
            
            // Usar reflection para setear la propiedad privada Department
            var departmentProperty = typeof(DepartmentHead).GetProperty("Department");
            departmentProperty?.SetValue(departmentHead, department);

            var consultationDerivation = new ConsultationDerivation(
                consultationDerivationId,
                "Diagnóstico test",
                Guid.NewGuid(),
                DateTime.Now,
                Guid.NewGuid(),
                departmentHead.DepartmentHeadId
            );

            // Usar reflection para setear DepartmentHead
            var deptHeadProperty = typeof(ConsultationDerivation).GetProperty("DepartmentHead");
            deptHeadProperty?.SetValue(consultationDerivation, departmentHead);

            var stock = new StockDepartment(
                stockId,
                50, // Cantidad inicial
                departmentId,
                medicationId,
                10,
                100
            );

            var medicationDerivation = new MedicationDerivation(
                Guid.NewGuid(),
                createDto.Quantity,
                consultationDerivationId,
                medicationId
            );

            var medicationDerivationDto = new MedicationDerivationDto
            {
                MedicationDerivationId = medicationDerivation.MedicationDerivationId,
                Quantity = createDto.Quantity
            };

            _mockConsultationDerivationRepository
                .Setup(r => r.GetWithDepartmentAsync(consultationDerivationId))
                .ReturnsAsync(consultationDerivation);

            _mockStockDepartmentRepository
                .Setup(r => r.GetByDepartmentAndMedicationAsync(departmentId, medicationId))
                .ReturnsAsync(stock);

            _mockStockDepartmentRepository
                .Setup(r => r.UpdateAsync(It.IsAny<StockDepartment>()))
                .Returns(Task.CompletedTask);

            _mockRepository
                .Setup(r => r.AddAsync(It.IsAny<MedicationDerivation>()))
                .ReturnsAsync(medicationDerivation);

            _mockMapper
                .Setup(m => m.Map<MedicationDerivationDto>(It.IsAny<MedicationDerivation>()))
                .Returns(medicationDerivationDto);

            // ACT
            var result = await _service.CreateAsync(createDto);

            // ASSERT
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(createDto.Quantity, result.Value.Quantity);
            
            // Verificar que se actualizó el stock (50 - 10 = 40)
            _mockStockDepartmentRepository.Verify(
                r => r.UpdateAsync(It.Is<StockDepartment>(s => s.Quantity == 40)), 
                Times.Once
            );
            
            _mockRepository.Verify(r => r.AddAsync(It.IsAny<MedicationDerivation>()), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_ConsultationDerivationNotFound_ReturnsFailure()
        {
            // ARRANGE
            var createDto = new CreateMedicationDerivationDto
            {
                Quantity = 10,
                ConsultationDerivationId = Guid.NewGuid(),
                MedicationId = Guid.NewGuid()
            };

            _mockConsultationDerivationRepository
                .Setup(r => r.GetWithDepartmentAsync(createDto.ConsultationDerivationId))
                .ReturnsAsync((ConsultationDerivation?)null);

            // ACT
            var result = await _service.CreateAsync(createDto);

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("no fue encontrada", result.ErrorMessage);
            _mockRepository.Verify(r => r.AddAsync(It.IsAny<MedicationDerivation>()), Times.Never);
        }

        [Fact]
        public async Task CreateAsync_DepartmentNotFound_ReturnsFailure()
        {
            // ARRANGE
            var createDto = new CreateMedicationDerivationDto
            {
                Quantity = 10,
                ConsultationDerivationId = Guid.NewGuid(),
                MedicationId = Guid.NewGuid()
            };

            var consultationDerivation = new ConsultationDerivation(
                createDto.ConsultationDerivationId,
                "Diagnóstico",
                Guid.NewGuid(),
                DateTime.Now,
                Guid.NewGuid(),
                Guid.NewGuid()
            );

            _mockConsultationDerivationRepository
                .Setup(r => r.GetWithDepartmentAsync(createDto.ConsultationDerivationId))
                .ReturnsAsync(consultationDerivation);

            // ACT
            var result = await _service.CreateAsync(createDto);

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("No se pudo obtener el departamento", result.ErrorMessage);
            _mockRepository.Verify(r => r.AddAsync(It.IsAny<MedicationDerivation>()), Times.Never);
        }

        [Fact]
        public async Task CreateAsync_StockNotFound_ReturnsFailure()
        {
            // ARRANGE
            var consultationDerivationId = Guid.NewGuid();
            var medicationId = Guid.NewGuid();
            var departmentId = Guid.NewGuid();

            var createDto = new CreateMedicationDerivationDto
            {
                Quantity = 10,
                ConsultationDerivationId = consultationDerivationId,
                MedicationId = medicationId
            };

            var department = new Department(departmentId, "Cardiología");
            var departmentHead = new DepartmentHead(Guid.NewGuid(), Guid.NewGuid(), departmentId, DateTime.UtcNow);
            
            var departmentProperty = typeof(DepartmentHead).GetProperty("Department");
            departmentProperty?.SetValue(departmentHead, department);

            var consultationDerivation = new ConsultationDerivation(
                consultationDerivationId,
                "Diagnóstico",
                Guid.NewGuid(),
                DateTime.Now,
                Guid.NewGuid(),
                departmentHead.DepartmentHeadId
            );

            var deptHeadProperty = typeof(ConsultationDerivation).GetProperty("DepartmentHead");
            deptHeadProperty?.SetValue(consultationDerivation, departmentHead);

            _mockConsultationDerivationRepository
                .Setup(r => r.GetWithDepartmentAsync(consultationDerivationId))
                .ReturnsAsync(consultationDerivation);

            _mockStockDepartmentRepository
                .Setup(r => r.GetByDepartmentAndMedicationAsync(departmentId, medicationId))
                .ReturnsAsync((StockDepartment?)null);

            // ACT
            var result = await _service.CreateAsync(createDto);

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("No existe stock del medicamento", result.ErrorMessage);
            _mockRepository.Verify(r => r.AddAsync(It.IsAny<MedicationDerivation>()), Times.Never);
        }

        [Fact]
        public async Task CreateAsync_InsufficientStock_ReturnsFailure()
        {
            // ARRANGE
            var consultationDerivationId = Guid.NewGuid();
            var medicationId = Guid.NewGuid();
            var departmentId = Guid.NewGuid();

            var createDto = new CreateMedicationDerivationDto
            {
                Quantity = 100, // Solicita más de lo disponible
                ConsultationDerivationId = consultationDerivationId,
                MedicationId = medicationId
            };

            var department = new Department(departmentId, "Cardiología");
            var departmentHead = new DepartmentHead(Guid.NewGuid(), Guid.NewGuid(), departmentId, DateTime.UtcNow);
            
            var departmentProperty = typeof(DepartmentHead).GetProperty("Department");
            departmentProperty?.SetValue(departmentHead, department);

            var consultationDerivation = new ConsultationDerivation(
                consultationDerivationId,
                "Diagnóstico",
                Guid.NewGuid(),
                DateTime.Now,
                Guid.NewGuid(),
                departmentHead.DepartmentHeadId
            );

            var deptHeadProperty = typeof(ConsultationDerivation).GetProperty("DepartmentHead");
            deptHeadProperty?.SetValue(consultationDerivation, departmentHead);

            var stock = new StockDepartment(
                Guid.NewGuid(),
                50, // Solo tiene 50 disponibles
                departmentId,
                medicationId,
                10,
                100
            );

            _mockConsultationDerivationRepository
                .Setup(r => r.GetWithDepartmentAsync(consultationDerivationId))
                .ReturnsAsync(consultationDerivation);

            _mockStockDepartmentRepository
                .Setup(r => r.GetByDepartmentAndMedicationAsync(departmentId, medicationId))
                .ReturnsAsync(stock);

            // ACT
            var result = await _service.CreateAsync(createDto);

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("Stock insuficiente", result.ErrorMessage);
            Assert.Contains("Disponible: 50", result.ErrorMessage);
            Assert.Contains("Solicitado: 100", result.ErrorMessage);
            _mockRepository.Verify(r => r.AddAsync(It.IsAny<MedicationDerivation>()), Times.Never);
        }

        [Fact]
        public async Task CreateAsync_RepositoryThrowsException_ReturnsFailure()
        {
            // ARRANGE
            var createDto = new CreateMedicationDerivationDto
            {
                Quantity = 10,
                ConsultationDerivationId = Guid.NewGuid(),
                MedicationId = Guid.NewGuid()
            };

            _mockConsultationDerivationRepository
                .Setup(r => r.GetWithDepartmentAsync(createDto.ConsultationDerivationId))
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
        public async Task GetByIdAsync_ValidId_ReturnsSuccessWithEntity()
        {
            // ARRANGE
            var medicationDerivationId = Guid.NewGuid();
            var medicationDerivation = new MedicationDerivation(
                medicationDerivationId,
                10,
                Guid.NewGuid(),
                Guid.NewGuid()
            );

            var medicationDerivationDto = new MedicationDerivationDto
            {
                MedicationDerivationId = medicationDerivationId,
                Quantity = 10
            };

            _mockRepository
                .Setup(r => r.GetByIdAsync(medicationDerivationId))
                .ReturnsAsync(medicationDerivation);

            _mockMapper
                .Setup(m => m.Map<MedicationDerivationDto>(medicationDerivation))
                .Returns(medicationDerivationDto);

            // ACT
            var result = await _service.GetByIdAsync(medicationDerivationId);

            // ASSERT
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(medicationDerivationId, result.Value.MedicationDerivationId);
            Assert.Equal(10, result.Value.Quantity);
        }

        [Fact]
        public async Task GetByIdAsync_EntityNotFound_ReturnsFailure()
        {
            // ARRANGE
            var medicationDerivationId = Guid.NewGuid();

            _mockRepository
                .Setup(r => r.GetByIdAsync(medicationDerivationId))
                .ReturnsAsync((MedicationDerivation?)null);

            // ACT
            var result = await _service.GetByIdAsync(medicationDerivationId);

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("no fue encontrada", result.ErrorMessage);
        }

        [Fact]
        public async Task GetByIdAsync_RepositoryThrowsException_ReturnsFailure()
        {
            // ARRANGE
            var medicationDerivationId = Guid.NewGuid();

            _mockRepository
                .Setup(r => r.GetByIdAsync(medicationDerivationId))
                .ThrowsAsync(new Exception("Database error"));

            // ACT
            var result = await _service.GetByIdAsync(medicationDerivationId);

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("Error", result.ErrorMessage);
        }

        #endregion

        #region GetAllAsync Tests

        [Fact]
        public async Task GetAllAsync_EntitiesExist_ReturnsSuccessWithList()
        {
            // ARRANGE
            var medicationDerivations = new List<MedicationDerivation>
            {
                new MedicationDerivation(Guid.NewGuid(), 10, Guid.NewGuid(), Guid.NewGuid()),
                new MedicationDerivation(Guid.NewGuid(), 20, Guid.NewGuid(), Guid.NewGuid()),
                new MedicationDerivation(Guid.NewGuid(), 30, Guid.NewGuid(), Guid.NewGuid())
            };

            var medicationDerivationDtos = new List<MedicationDerivationDto>
            {
                new MedicationDerivationDto { MedicationDerivationId = medicationDerivations[0].MedicationDerivationId, Quantity = 10 },
                new MedicationDerivationDto { MedicationDerivationId = medicationDerivations[1].MedicationDerivationId, Quantity = 20 },
                new MedicationDerivationDto { MedicationDerivationId = medicationDerivations[2].MedicationDerivationId, Quantity = 30 }
            };

            _mockRepository
                .Setup(r => r.GetAllAsync())
                .ReturnsAsync(medicationDerivations);

            _mockMapper
                .Setup(m => m.Map<IEnumerable<MedicationDerivationDto>>(medicationDerivations))
                .Returns(medicationDerivationDtos);

            // ACT
            var result = await _service.GetAllAsync();

            // ASSERT
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(3, result.Value.Count());
        }

        [Fact]
        public async Task GetAllAsync_NoEntities_ReturnsSuccessWithEmptyList()
        {
            // ARRANGE
            var emptyList = new List<MedicationDerivation>();

            _mockRepository
                .Setup(r => r.GetAllAsync())
                .ReturnsAsync(emptyList);

            _mockMapper
                .Setup(m => m.Map<IEnumerable<MedicationDerivationDto>>(emptyList))
                .Returns(new List<MedicationDerivationDto>());

            // ACT
            var result = await _service.GetAllAsync();

            // ASSERT
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Empty(result.Value);
        }

        [Fact]
        public async Task GetAllAsync_RepositoryThrowsException_ReturnsFailure()
        {
            // ARRANGE
            _mockRepository
                .Setup(r => r.GetAllAsync())
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
        public async Task UpdateAsync_IncreaseQuantity_DecreasesStockCorrectly()
        {
            // ARRANGE
            var medicationDerivationId = Guid.NewGuid();
            var consultationDerivationId = Guid.NewGuid();
            var medicationId = Guid.NewGuid();
            var departmentId = Guid.NewGuid();

            var updateDto = new UpdateMedicationDerivationDto
            {
                Quantity = 20 // Aumenta de 10 a 20
            };

            var medicationDerivation = new MedicationDerivation(
                medicationDerivationId,
                10, // Cantidad original
                consultationDerivationId,
                medicationId
            );

            var department = new Department(departmentId, "Cardiología");
            var departmentHead = new DepartmentHead(Guid.NewGuid(), Guid.NewGuid(), departmentId, DateTime.UtcNow);
            
            var departmentProperty = typeof(DepartmentHead).GetProperty("Department");
            departmentProperty?.SetValue(departmentHead, department);

            var consultationDerivation = new ConsultationDerivation(
                consultationDerivationId,
                "Diagnóstico",
                Guid.NewGuid(),
                DateTime.Now,
                Guid.NewGuid(),
                departmentHead.DepartmentHeadId
            );

            var deptHeadProperty = typeof(ConsultationDerivation).GetProperty("DepartmentHead");
            deptHeadProperty?.SetValue(consultationDerivation, departmentHead);

            var stock = new StockDepartment(
                Guid.NewGuid(),
                50, // Stock disponible
                departmentId,
                medicationId,
                10,
                100
            );

            _mockRepository
                .Setup(r => r.GetByIdAsync(medicationDerivationId))
                .ReturnsAsync(medicationDerivation);

            _mockConsultationDerivationRepository
                .Setup(r => r.GetWithDepartmentAsync(consultationDerivationId))
                .ReturnsAsync(consultationDerivation);

            _mockStockDepartmentRepository
                .Setup(r => r.GetByDepartmentAndMedicationAsync(departmentId, medicationId))
                .ReturnsAsync(stock);

            _mockStockDepartmentRepository
                .Setup(r => r.UpdateAsync(It.IsAny<StockDepartment>()))
                .Returns(Task.CompletedTask);

            _mockRepository
                .Setup(r => r.UpdateAsync(It.IsAny<MedicationDerivation>()))
                .Returns(Task.CompletedTask);

            // ACT
            var result = await _service.UpdateAsync(medicationDerivationId, updateDto);

            // ASSERT
            Assert.True(result.IsSuccess);
            Assert.True(result.Value);
            
            // Verificar que el stock se redujo en 10 (diferencia: 20 - 10 = 10)
            // Stock final: 50 - 10 = 40
            _mockStockDepartmentRepository.Verify(
                r => r.UpdateAsync(It.Is<StockDepartment>(s => s.Quantity == 40)),
                Times.Once
            );

            _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<MedicationDerivation>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_DecreaseQuantity_IncreasesStockCorrectly()
        {
            // ARRANGE
            var medicationDerivationId = Guid.NewGuid();
            var consultationDerivationId = Guid.NewGuid();
            var medicationId = Guid.NewGuid();
            var departmentId = Guid.NewGuid();

            var updateDto = new UpdateMedicationDerivationDto
            {
                Quantity = 5 // Disminuye de 10 a 5
            };

            var medicationDerivation = new MedicationDerivation(
                medicationDerivationId,
                10, // Cantidad original
                consultationDerivationId,
                medicationId
            );

            var department = new Department(departmentId, "Cardiología");
            var departmentHead = new DepartmentHead(Guid.NewGuid(), Guid.NewGuid(), departmentId, DateTime.UtcNow);
            
            var departmentProperty = typeof(DepartmentHead).GetProperty("Department");
            departmentProperty?.SetValue(departmentHead, department);

            var consultationDerivation = new ConsultationDerivation(
                consultationDerivationId,
                "Diagnóstico",
                Guid.NewGuid(),
                DateTime.Now,
                Guid.NewGuid(),
                departmentHead.DepartmentHeadId
            );

            var deptHeadProperty = typeof(ConsultationDerivation).GetProperty("DepartmentHead");
            deptHeadProperty?.SetValue(consultationDerivation, departmentHead);

            var stock = new StockDepartment(
                Guid.NewGuid(),
                50, // Stock disponible
                departmentId,
                medicationId,
                10,
                100
            );

            _mockRepository
                .Setup(r => r.GetByIdAsync(medicationDerivationId))
                .ReturnsAsync(medicationDerivation);

            _mockConsultationDerivationRepository
                .Setup(r => r.GetWithDepartmentAsync(consultationDerivationId))
                .ReturnsAsync(consultationDerivation);

            _mockStockDepartmentRepository
                .Setup(r => r.GetByDepartmentAndMedicationAsync(departmentId, medicationId))
                .ReturnsAsync(stock);

            _mockStockDepartmentRepository
                .Setup(r => r.UpdateAsync(It.IsAny<StockDepartment>()))
                .Returns(Task.CompletedTask);

            _mockRepository
                .Setup(r => r.UpdateAsync(It.IsAny<MedicationDerivation>()))
                .Returns(Task.CompletedTask);

            // ACT
            var result = await _service.UpdateAsync(medicationDerivationId, updateDto);

            // ASSERT
            Assert.True(result.IsSuccess);
            Assert.True(result.Value);
            
            // Verificar que el stock aumentó en 5 (diferencia: 5 - 10 = -5)
            // Stock final: 50 + 5 = 55
            _mockStockDepartmentRepository.Verify(
                r => r.UpdateAsync(It.Is<StockDepartment>(s => s.Quantity == 55)),
                Times.Once
            );

            _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<MedicationDerivation>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_EntityNotFound_ReturnsFailure()
        {
            // ARRANGE
            var medicationDerivationId = Guid.NewGuid();
            var updateDto = new UpdateMedicationDerivationDto { Quantity = 20 };

            _mockRepository
                .Setup(r => r.GetByIdAsync(medicationDerivationId))
                .ReturnsAsync((MedicationDerivation?)null);

            // ACT
            var result = await _service.UpdateAsync(medicationDerivationId, updateDto);

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("no fue encontrada", result.ErrorMessage);
            _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<MedicationDerivation>()), Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_InsufficientStockForIncrease_ReturnsFailure()
        {
            // ARRANGE
            var medicationDerivationId = Guid.NewGuid();
            var consultationDerivationId = Guid.NewGuid();
            var medicationId = Guid.NewGuid();
            var departmentId = Guid.NewGuid();

            var updateDto = new UpdateMedicationDerivationDto
            {
                Quantity = 100 // Intenta aumentar mucho más
            };

            var medicationDerivation = new MedicationDerivation(
                medicationDerivationId,
                10,
                consultationDerivationId,
                medicationId
            );

            var department = new Department(departmentId, "Cardiología");
            var departmentHead = new DepartmentHead(Guid.NewGuid(), Guid.NewGuid(), departmentId, DateTime.UtcNow);
            
            var departmentProperty = typeof(DepartmentHead).GetProperty("Department");
            departmentProperty?.SetValue(departmentHead, department);

            var consultationDerivation = new ConsultationDerivation(
                consultationDerivationId,
                "Diagnóstico",
                Guid.NewGuid(),
                DateTime.Now,
                Guid.NewGuid(),
                departmentHead.DepartmentHeadId
            );

            var deptHeadProperty = typeof(ConsultationDerivation).GetProperty("DepartmentHead");
            deptHeadProperty?.SetValue(consultationDerivation, departmentHead);

            var stock = new StockDepartment(
                Guid.NewGuid(),
                20, // Solo hay 20 disponibles, pero necesita 90 adicionales
                departmentId,
                medicationId,
                10,
                100
            );

            _mockRepository
                .Setup(r => r.GetByIdAsync(medicationDerivationId))
                .ReturnsAsync(medicationDerivation);

            _mockConsultationDerivationRepository
                .Setup(r => r.GetWithDepartmentAsync(consultationDerivationId))
                .ReturnsAsync(consultationDerivation);

            _mockStockDepartmentRepository
                .Setup(r => r.GetByDepartmentAndMedicationAsync(departmentId, medicationId))
                .ReturnsAsync(stock);

            // ACT
            var result = await _service.UpdateAsync(medicationDerivationId, updateDto);

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("Stock insuficiente", result.ErrorMessage);
            _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<MedicationDerivation>()), Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_UpdateOtherFields_UpdatesSuccessfully()
        {
            // ARRANGE
            var medicationDerivationId = Guid.NewGuid();
            var newConsultationDerivationId = Guid.NewGuid();
            var newMedicationId = Guid.NewGuid();

            var updateDto = new UpdateMedicationDerivationDto
            {
                ConsultationDerivationId = newConsultationDerivationId,
                MedicationId = newMedicationId
                // Sin cambio de cantidad
            };

            var medicationDerivation = new MedicationDerivation(
                medicationDerivationId,
                10,
                Guid.NewGuid(),
                Guid.NewGuid()
            );

            _mockRepository
                .Setup(r => r.GetByIdAsync(medicationDerivationId))
                .ReturnsAsync(medicationDerivation);

            _mockRepository
                .Setup(r => r.UpdateAsync(It.IsAny<MedicationDerivation>()))
                .Returns(Task.CompletedTask);

            // ACT
            var result = await _service.UpdateAsync(medicationDerivationId, updateDto);

            // ASSERT
            Assert.True(result.IsSuccess);
            Assert.True(result.Value);
            _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<MedicationDerivation>()), Times.Once);
            
            // No debe llamar a stock si no hay cambio de cantidad
            _mockStockDepartmentRepository.Verify(
                r => r.UpdateAsync(It.IsAny<StockDepartment>()), 
                Times.Never
            );
        }

        [Fact]
        public async Task UpdateAsync_RepositoryThrowsException_ReturnsFailure()
        {
            // ARRANGE
            var medicationDerivationId = Guid.NewGuid();
            var updateDto = new UpdateMedicationDerivationDto { Quantity = 20 };

            _mockRepository
                .Setup(r => r.GetByIdAsync(medicationDerivationId))
                .ThrowsAsync(new Exception("Database error"));

            // ACT
            var result = await _service.UpdateAsync(medicationDerivationId, updateDto);

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("Error", result.ErrorMessage);
        }

        #endregion

        #region DeleteAsync Tests

        [Fact]
        public async Task DeleteAsync_ValidId_DeletesAndRestoresStock()
        {
            // ARRANGE
            var medicationDerivationId = Guid.NewGuid();
            var consultationDerivationId = Guid.NewGuid();
            var medicationId = Guid.NewGuid();
            var departmentId = Guid.NewGuid();

            var medicationDerivation = new MedicationDerivation(
                medicationDerivationId,
                15, // Cantidad a devolver al stock
                consultationDerivationId,
                medicationId
            );

            var department = new Department(departmentId, "Cardiología");
            var departmentHead = new DepartmentHead(Guid.NewGuid(), Guid.NewGuid(), departmentId, DateTime.UtcNow);
            
            var departmentProperty = typeof(DepartmentHead).GetProperty("Department");
            departmentProperty?.SetValue(departmentHead, department);

            var consultationDerivation = new ConsultationDerivation(
                consultationDerivationId,
                "Diagnóstico",
                Guid.NewGuid(),
                DateTime.Now,
                Guid.NewGuid(),
                departmentHead.DepartmentHeadId
            );

            var deptHeadProperty = typeof(ConsultationDerivation).GetProperty("DepartmentHead");
            deptHeadProperty?.SetValue(consultationDerivation, departmentHead);

            var stock = new StockDepartment(
                Guid.NewGuid(),
                35, // Stock actual
                departmentId,
                medicationId,
                10,
                100
            );

            _mockRepository
                .Setup(r => r.GetByIdAsync(medicationDerivationId))
                .ReturnsAsync(medicationDerivation);

            _mockConsultationDerivationRepository
                .Setup(r => r.GetWithDepartmentAsync(consultationDerivationId))
                .ReturnsAsync(consultationDerivation);

            _mockStockDepartmentRepository
                .Setup(r => r.GetByDepartmentAndMedicationAsync(departmentId, medicationId))
                .ReturnsAsync(stock);

            _mockStockDepartmentRepository
                .Setup(r => r.UpdateAsync(It.IsAny<StockDepartment>()))
                .Returns(Task.CompletedTask);

            _mockRepository
                .Setup(r => r.DeleteAsync(It.IsAny<MedicationDerivation>()))
                .Returns(Task.CompletedTask);

            // ACT
            var result = await _service.DeleteAsync(medicationDerivationId);

            // ASSERT
            Assert.True(result.IsSuccess);
            Assert.True(result.Value);

            // Verificar que el stock se restauró (35 + 15 = 50)
            _mockStockDepartmentRepository.Verify(
                r => r.UpdateAsync(It.Is<StockDepartment>(s => s.Quantity == 50)),
                Times.Once
            );

            _mockRepository.Verify(r => r.DeleteAsync(It.IsAny<MedicationDerivation>()), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_EntityNotFound_ReturnsFailure()
        {
            // ARRANGE
            var medicationDerivationId = Guid.NewGuid();

            _mockRepository
                .Setup(r => r.GetByIdAsync(medicationDerivationId))
                .ReturnsAsync((MedicationDerivation?)null);

            // ACT
            var result = await _service.DeleteAsync(medicationDerivationId);

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("no fue encontrada", result.ErrorMessage);
            _mockRepository.Verify(r => r.DeleteAsync(It.IsAny<MedicationDerivation>()), Times.Never);
        }

        [Fact]
        public async Task DeleteAsync_StockNotFound_StillDeletesEntity()
        {
            // ARRANGE
            var medicationDerivationId = Guid.NewGuid();
            var consultationDerivationId = Guid.NewGuid();
            var medicationId = Guid.NewGuid();
            var departmentId = Guid.NewGuid();

            var medicationDerivation = new MedicationDerivation(
                medicationDerivationId,
                10,
                consultationDerivationId,
                medicationId
            );

            var department = new Department(departmentId, "Cardiología");
            var departmentHead = new DepartmentHead(Guid.NewGuid(), Guid.NewGuid(), departmentId, DateTime.UtcNow);
            
            var departmentProperty = typeof(DepartmentHead).GetProperty("Department");
            departmentProperty?.SetValue(departmentHead, department);

            var consultationDerivation = new ConsultationDerivation(
                consultationDerivationId,
                "Diagnóstico",
                Guid.NewGuid(),
                DateTime.Now,
                Guid.NewGuid(),
                departmentHead.DepartmentHeadId
            );

            var deptHeadProperty = typeof(ConsultationDerivation).GetProperty("DepartmentHead");
            deptHeadProperty?.SetValue(consultationDerivation, departmentHead);

            _mockRepository
                .Setup(r => r.GetByIdAsync(medicationDerivationId))
                .ReturnsAsync(medicationDerivation);

            _mockConsultationDerivationRepository
                .Setup(r => r.GetWithDepartmentAsync(consultationDerivationId))
                .ReturnsAsync(consultationDerivation);

            _mockStockDepartmentRepository
                .Setup(r => r.GetByDepartmentAndMedicationAsync(departmentId, medicationId))
                .ReturnsAsync((StockDepartment?)null); // Stock no encontrado

            _mockRepository
                .Setup(r => r.DeleteAsync(It.IsAny<MedicationDerivation>()))
                .Returns(Task.CompletedTask);

            // ACT
            var result = await _service.DeleteAsync(medicationDerivationId);

            // ASSERT
            Assert.True(result.IsSuccess);
            Assert.True(result.Value);
            
            // No debe actualizar stock si no existe
            _mockStockDepartmentRepository.Verify(
                r => r.UpdateAsync(It.IsAny<StockDepartment>()),
                Times.Never
            );

            // Pero sí debe eliminar la entidad
            _mockRepository.Verify(r => r.DeleteAsync(It.IsAny<MedicationDerivation>()), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ConsultationDerivationNotFound_StillDeletesEntity()
        {
            // ARRANGE
            var medicationDerivationId = Guid.NewGuid();

            var medicationDerivation = new MedicationDerivation(
                medicationDerivationId,
                10,
                Guid.NewGuid(),
                Guid.NewGuid()
            );

            _mockRepository
                .Setup(r => r.GetByIdAsync(medicationDerivationId))
                .ReturnsAsync(medicationDerivation);

            _mockConsultationDerivationRepository
                .Setup(r => r.GetWithDepartmentAsync(medicationDerivation.ConsultationDerivationId))
                .ReturnsAsync((ConsultationDerivation?)null);

            _mockRepository
                .Setup(r => r.DeleteAsync(It.IsAny<MedicationDerivation>()))
                .Returns(Task.CompletedTask);

            // ACT
            var result = await _service.DeleteAsync(medicationDerivationId);

            // ASSERT
            Assert.True(result.IsSuccess);
            Assert.True(result.Value);
            
            // No debe intentar actualizar stock
            _mockStockDepartmentRepository.Verify(
                r => r.UpdateAsync(It.IsAny<StockDepartment>()),
                Times.Never
            );

            // Pero sí debe eliminar la entidad
            _mockRepository.Verify(r => r.DeleteAsync(It.IsAny<MedicationDerivation>()), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_RepositoryThrowsException_ReturnsFailure()
        {
            // ARRANGE
            var medicationDerivationId = Guid.NewGuid();

            _mockRepository
                .Setup(r => r.GetByIdAsync(medicationDerivationId))
                .ThrowsAsync(new Exception("Database error"));

            // ACT
            var result = await _service.DeleteAsync(medicationDerivationId);

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("Error", result.ErrorMessage);
        }

        #endregion
    }
}
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
    public class MedicationServiceTests
    {
        private readonly Mock<IMedicationRepository> _mockRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly MedicationService _medicationService;

        public MedicationServiceTests()
        {
            _mockRepository = new Mock<IMedicationRepository>();
            _mockMapper = new Mock<IMapper>();
            _medicationService = new MedicationService(_mockRepository.Object, _mockMapper.Object);
        }

        #region CreateAsync Tests

        [Fact]
        public async Task CreateAsync_ValidMedication_ReturnsSuccessResult()
        {
            // ARRANGE
            var createDto = new CreateMedicationDto
            {
                Format = "Tableta",
                CommercialName = "Paracetamol 500mg",
                CommercialCompany = "Bayer",
                BatchNumber = "BATCH001",
                ScientificName = "Acetaminofén",
                ExpirationDate = "2025-12-31",
                QuantityWarehouse = 100,
                QuantityNurse = 20,
                MinQuantityWarehouse = 10,
                MinQuantityNurse = 5,
                MaxQuantityWarehouse = 500,
                MaxQuantityNurse = 100
            };

            var medication = new Medication(
                Guid.NewGuid(),
                createDto.Format,
                createDto.CommercialName,
                createDto.CommercialCompany,
                createDto.BatchNumber,
                createDto.ScientificName,
                DateOnly.ParseExact(createDto.ExpirationDate, "yyyy-MM-dd"),
                createDto.QuantityWarehouse,
                createDto.QuantityNurse,
                createDto.MinQuantityWarehouse,
                createDto.MinQuantityNurse,
                createDto.MaxQuantityWarehouse,
                createDto.MaxQuantityNurse
            );

            var medicationDto = new MedicationDto
            {
                MedicationId = medication.MedicationId,
                CommercialName = medication.CommercialName
            };

            _mockRepository.Setup(r => r.ExistsBatchAsync(createDto.BatchNumber)).ReturnsAsync(false);
            _mockRepository.Setup(r => r.ExistsMedicationAsync(createDto.CommercialName)).ReturnsAsync(false);
            _mockRepository.Setup(r => r.ExistsMedicationAsync(createDto.ScientificName)).ReturnsAsync(false);
            _mockRepository.Setup(r => r.AddAsync(It.IsAny<Medication>())).ReturnsAsync(medication);
            _mockMapper.Setup(m => m.Map<MedicationDto>(It.IsAny<Medication>())).Returns(medicationDto);

            // ACT
            var result = await _medicationService.CreateAsync(createDto);

            // ASSERT
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(createDto.CommercialName, result.Value.CommercialName);
            _mockRepository.Verify(r => r.AddAsync(It.IsAny<Medication>()), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_DuplicateBatchNumber_ReturnsFailureResult()
        {
            // ARRANGE
            var createDto = new CreateMedicationDto
            {
                BatchNumber = "BATCH001",
                CommercialName = "Test",
                ScientificName = "TestScientific",
                ExpirationDate = "2025-12-31"
            };

            _mockRepository.Setup(r => r.ExistsBatchAsync(createDto.BatchNumber)).ReturnsAsync(true);
            _mockRepository.Setup(r => r.ExistsMedicationAsync(createDto.CommercialName)).ReturnsAsync(true);
            _mockRepository.Setup(r => r.ExistsMedicationAsync(createDto.ScientificName)).ReturnsAsync(true);

            // ACT
            var result = await _medicationService.CreateAsync(createDto);

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("Ya existe un medicamento con este número de lote", result.ErrorMessage);
            _mockRepository.Verify(r => r.AddAsync(It.IsAny<Medication>()), Times.Never);
        }

        [Fact]
        public async Task CreateAsync_RepositoryThrowsException_ReturnsFailureResult()
        {
            // ARRANGE
            var createDto = new CreateMedicationDto
            {
                BatchNumber = "BATCH001",
                CommercialName = "Test",
                ScientificName = "TestScientific",
                ExpirationDate = "2025-12-31"
            };

            _mockRepository.Setup(r => r.ExistsBatchAsync(createDto.BatchNumber)).ReturnsAsync(false);
            _mockRepository.Setup(r => r.ExistsMedicationAsync(It.IsAny<string>())).ReturnsAsync(false);
            _mockRepository.Setup(r => r.AddAsync(It.IsAny<Medication>())).ThrowsAsync(new Exception("Database error"));

            // ACT
            var result = await _medicationService.CreateAsync(createDto);

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("Error al guardar el medicamento", result.ErrorMessage);
        }

        #endregion

        #region GetAllAsync Tests

        [Fact]
        public async Task GetAllAsync_MedicationsExist_ReturnsSuccessWithList()
        {
            // ARRANGE
            var medications = new List<Medication>
            {
                new Medication(Guid.NewGuid(), "Tableta", "Med1", "Company1", "BATCH001", "Sci1", 
                    DateOnly.FromDateTime(DateTime.Now.AddYears(1)), 100, 20, 10, 5, 500, 100),
                new Medication(Guid.NewGuid(), "Jarabe", "Med2", "Company2", "BATCH002", "Sci2", 
                    DateOnly.FromDateTime(DateTime.Now.AddYears(1)), 50, 10, 10, 5, 300, 50)
            };

            var medicationDtos = new List<MedicationDto>
            {
                new MedicationDto { MedicationId = medications[0].MedicationId, CommercialName = "Med1" },
                new MedicationDto { MedicationId = medications[1].MedicationId, CommercialName = "Med2" }
            };

            _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(medications);
            _mockMapper.Setup(m => m.Map<IEnumerable<MedicationDto>>(medications)).Returns(medicationDtos);

            // ACT
            var result = await _medicationService.GetAllAsync();

            // ASSERT
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(2, result.Value.Count());
            _mockRepository.Verify(r => r.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetAllAsync_NoMedications_ReturnsSuccessWithEmptyList()
        {
            // ARRANGE
            var emptyList = new List<Medication>();
            _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(emptyList);
            _mockMapper.Setup(m => m.Map<IEnumerable<MedicationDto>>(emptyList)).Returns(new List<MedicationDto>());

            // ACT
            var result = await _medicationService.GetAllAsync();

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
            var result = await _medicationService.GetAllAsync();

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("Error al obtener medicamentos", result.ErrorMessage);
        }

        #endregion

        #region GetByIdAsync Tests

        [Fact]
        public async Task GetByIdAsync_ValidId_ReturnsSuccessWithMedication()
        {
            // ARRANGE
            var medicationId = Guid.NewGuid();
            var medication = new Medication(
                medicationId, "Tableta", "Paracetamol", "Bayer", "BATCH001", "Acetaminofén",
                DateOnly.FromDateTime(DateTime.Now.AddYears(1)), 100, 20, 10, 5, 500, 100
            );
            var medicationDto = new MedicationDto { MedicationId = medicationId, CommercialName = "Paracetamol" };

            _mockRepository.Setup(r => r.GetByIdAsync(medicationId)).ReturnsAsync(medication);
            _mockMapper.Setup(m => m.Map<MedicationDto>(medication)).Returns(medicationDto);

            // ACT
            var result = await _medicationService.GetByIdAsync(medicationId);

            // ASSERT
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(medicationId, result.Value.MedicationId);
            Assert.Equal("Paracetamol", result.Value.CommercialName);
            _mockRepository.Verify(r => r.GetByIdAsync(medicationId), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_MedicationNotFound_ReturnsFailureResult()
        {
            // ARRANGE
            var medicationId = Guid.NewGuid();
            _mockRepository.Setup(r => r.GetByIdAsync(medicationId)).ReturnsAsync((Medication?)null);

            // ACT
            var result = await _medicationService.GetByIdAsync(medicationId);

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("Medicamento no encontrado", result.ErrorMessage);
        }

        [Fact]
        public async Task GetByIdAsync_RepositoryThrowsException_ReturnsFailureResult()
        {
            // ARRANGE
            var medicationId = Guid.NewGuid();
            _mockRepository.Setup(r => r.GetByIdAsync(medicationId)).ThrowsAsync(new Exception("Database error"));

            // ACT
            var result = await _medicationService.GetByIdAsync(medicationId);

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("Error al obtener el medicamento", result.ErrorMessage);
        }

        #endregion

        #region UpdateAsync Tests

        [Fact]
        public async Task UpdateAsync_ValidUpdate_ReturnsSuccessResult()
        {
            // ARRANGE
            var medicationId = Guid.NewGuid();
            var existingMedication = new Medication(
                medicationId, "Tableta", "OldName", "OldCompany", "BATCH001", "OldScientific",
                DateOnly.FromDateTime(DateTime.Now.AddYears(1)), 100, 20, 10, 5, 500, 100
            );
            var updateDto = new UpdateMedicationDto
            {
                Format = "Jarabe",
                CommercialName = "NewName",
                CommercialCompany = "NewCompany",
                ExpirationDate = "2026-12-31",
                ScientificName = "NewScientific",
                QuantityWarehouse = 150
            };

            _mockRepository.Setup(r => r.GetByIdAsync(medicationId)).ReturnsAsync(existingMedication);
            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Medication>())).Returns(Task.CompletedTask);

            // ACT
            var result = await _medicationService.UpdateAsync(medicationId, updateDto);

            // ASSERT
            Assert.True(result.IsSuccess);
            Assert.True(result.Value);
            _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Medication>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_MedicationNotFound_ReturnsFailureResult()
        {
            // ARRANGE
            var medicationId = Guid.NewGuid();
            var updateDto = new UpdateMedicationDto { CommercialName = "NewName" };
            _mockRepository.Setup(r => r.GetByIdAsync(medicationId)).ReturnsAsync((Medication?)null);

            // ACT
            var result = await _medicationService.UpdateAsync(medicationId, updateDto);

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("Medicamento no encontrado", result.ErrorMessage);
            _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Medication>()), Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_RepositoryThrowsException_ReturnsFailureResult()
        {
            // ARRANGE
            var medicationId = Guid.NewGuid();
            var existingMedication = new Medication(
                medicationId, "Tableta", "OldName", "OldCompany", "BATCH001", "OldScientific",
                DateOnly.FromDateTime(DateTime.Now.AddYears(1)), 100, 20, 10, 5, 500, 100
            );
            var updateDto = new UpdateMedicationDto { CommercialName = "NewName" };

            _mockRepository.Setup(r => r.GetByIdAsync(medicationId)).ReturnsAsync(existingMedication);
            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Medication>())).ThrowsAsync(new Exception("Database error"));

            // ACT
            var result = await _medicationService.UpdateAsync(medicationId, updateDto);

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("Error al actualizar el medicamento", result.ErrorMessage);
        }

        #endregion

        #region DeleteAsync Tests

        [Fact]
        public async Task DeleteAsync_ValidId_ReturnsSuccessResult()
        {
            // ARRANGE
            var medicationId = Guid.NewGuid();
            var medication = new Medication(
                medicationId, "Tableta", "Paracetamol", "Bayer", "BATCH001", "Acetaminofén",
                DateOnly.FromDateTime(DateTime.Now.AddYears(1)), 100, 20, 10, 5, 500, 100
            );

            _mockRepository.Setup(r => r.GetByIdAsync(medicationId)).ReturnsAsync(medication);
            _mockRepository.Setup(r => r.DeleteAsync(medication)).Returns(Task.CompletedTask);

            // ACT
            var result = await _medicationService.DeleteAsync(medicationId);

            // ASSERT
            Assert.True(result.IsSuccess);
            Assert.True(result.Value);
            _mockRepository.Verify(r => r.GetByIdAsync(medicationId), Times.Once);
            _mockRepository.Verify(r => r.DeleteAsync(medication), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_MedicationNotFound_ReturnsFailureResult()
        {
            // ARRANGE
            var medicationId = Guid.NewGuid();
            _mockRepository.Setup(r => r.GetByIdAsync(medicationId)).ReturnsAsync((Medication?)null);

            // ACT
            var result = await _medicationService.DeleteAsync(medicationId);

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("Medicamento no encontrado", result.ErrorMessage);
            _mockRepository.Verify(r => r.DeleteAsync(It.IsAny<Medication>()), Times.Never);
        }

        [Fact]
        public async Task DeleteAsync_RepositoryThrowsException_ReturnsFailureResult()
        {
            // ARRANGE
            var medicationId = Guid.NewGuid();
            var medication = new Medication(
                medicationId, "Tableta", "Paracetamol", "Bayer", "BATCH001", "Acetaminofén",
                DateOnly.FromDateTime(DateTime.Now.AddYears(1)), 100, 20, 10, 5, 500, 100
            );

            _mockRepository.Setup(r => r.GetByIdAsync(medicationId)).ReturnsAsync(medication);
            _mockRepository.Setup(r => r.DeleteAsync(medication)).ThrowsAsync(new Exception("Database error"));

            // ACT
            var result = await _medicationService.DeleteAsync(medicationId);

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("Error al eliminar el medicamento", result.ErrorMessage);
        }

        #endregion

        #region GetByBatchNumberAsync Tests

        [Fact]
        public async Task GetByBatchNumberAsync_ValidBatchNumber_ReturnsSuccessWithMedication()
        {
            // ARRANGE
            var batchNumber = "BATCH001";
            var medication = new Medication(
                Guid.NewGuid(), "Tableta", "Paracetamol", "Bayer", batchNumber, "Acetaminofén",
                DateOnly.FromDateTime(DateTime.Now.AddYears(1)), 100, 20, 10, 5, 500, 100
            );
            var medicationDto = new MedicationDto { MedicationId = medication.MedicationId, BatchNumber = batchNumber };

            _mockRepository.Setup(r => r.GetByBatchNumberAsync(batchNumber)).ReturnsAsync(medication);
            _mockMapper.Setup(m => m.Map<MedicationDto>(medication)).Returns(medicationDto);

            // ACT
            var result = await _medicationService.GetByBatchNumberAsync(batchNumber);

            // ASSERT
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(batchNumber, result.Value.BatchNumber);
            _mockRepository.Verify(r => r.GetByBatchNumberAsync(batchNumber), Times.Once);
        }

        [Fact]
        public async Task GetByBatchNumberAsync_BatchNotFound_ReturnsFailureResult()
        {
            // ARRANGE
            var batchNumber = "NONEXISTENT";
            _mockRepository.Setup(r => r.GetByBatchNumberAsync(batchNumber)).ReturnsAsync((Medication?)null);

            // ACT
            var result = await _medicationService.GetByBatchNumberAsync(batchNumber);

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("No existe un medicamento con ese número de lote", result.ErrorMessage);
        }

        [Fact]
        public async Task GetByBatchNumberAsync_RepositoryThrowsException_ReturnsFailureResult()
        {
            // ARRANGE
            var batchNumber = "BATCH001";
            _mockRepository.Setup(r => r.GetByBatchNumberAsync(batchNumber)).ThrowsAsync(new Exception("Database error"));

            // ACT
            var result = await _medicationService.GetByBatchNumberAsync(batchNumber);

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("Error al buscar medicamento por lote", result.ErrorMessage);
        }

        #endregion

        #region GetByCommercialCompanyAsync Tests

        [Fact]
        public async Task GetByCommercialCompanyAsync_CompanyExists_ReturnsSuccessWithList()
        {
            // ARRANGE
            var company = "Bayer";
            var medications = new List<Medication>
            {
                new Medication(Guid.NewGuid(), "Tableta", "Med1", company, "BATCH001", "Sci1",
                    DateOnly.FromDateTime(DateTime.Now.AddYears(1)), 100, 20, 10, 5, 500, 100),
                new Medication(Guid.NewGuid(), "Jarabe", "Med2", company, "BATCH002", "Sci2",
                    DateOnly.FromDateTime(DateTime.Now.AddYears(1)), 50, 10, 10, 5, 300, 50)
            };

            var medicationDtos = new List<MedicationDto>
            {
                new MedicationDto { CommercialCompany = company },
                new MedicationDto { CommercialCompany = company }
            };

            _mockRepository.Setup(r => r.GetByCommercialCompanyAsync(company)).ReturnsAsync(medications);
            _mockMapper.Setup(m => m.Map<IEnumerable<MedicationDto>>(medications)).Returns(medicationDtos);

            // ACT
            var result = await _medicationService.GetByCommercialCompanyAsync(company);

            // ASSERT
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(2, result.Value.Count());
        }

        [Fact]
        public async Task GetByCommercialCompanyAsync_RepositoryThrowsException_ReturnsFailureResult()
        {
            // ARRANGE
            var company = "Bayer";
            _mockRepository.Setup(r => r.GetByCommercialCompanyAsync(company)).ThrowsAsync(new Exception("Database error"));

            // ACT
            var result = await _medicationService.GetByCommercialCompanyAsync(company);

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("Error al buscar medicamentos por compañía", result.ErrorMessage);
        }

        #endregion

        #region SearchByNameAsync Tests

        [Fact]
        public async Task SearchByNameAsync_NameExists_ReturnsSuccessWithList()
        {
            // ARRANGE
            var searchName = "Paracetamol";
            var medications = new List<Medication>
            {
                new Medication(Guid.NewGuid(), "Tableta", "Paracetamol 500mg", "Bayer", "BATCH001", "Acetaminofén",
                    DateOnly.FromDateTime(DateTime.Now.AddYears(1)), 100, 20, 10, 5, 500, 100)
            };

            var medicationDtos = new List<MedicationDto>
            {
                new MedicationDto { CommercialName = "Paracetamol 500mg" }
            };

            _mockRepository.Setup(r => r.SearchByNameAsync(searchName)).ReturnsAsync(medications);
            _mockMapper.Setup(m => m.Map<IEnumerable<MedicationDto>>(medications)).Returns(medicationDtos);

            // ACT
            var result = await _medicationService.SearchByNameAsync(searchName);

            // ASSERT
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Single(result.Value);
        }

        [Fact]
        public async Task SearchByNameAsync_RepositoryThrowsException_ReturnsFailureResult()
        {
            // ARRANGE
            var searchName = "Test";
            _mockRepository.Setup(r => r.SearchByNameAsync(searchName)).ThrowsAsync(new Exception("Database error"));

            // ACT
            var result = await _medicationService.SearchByNameAsync(searchName);

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("Error al buscar medicamentos por nombre", result.ErrorMessage);
        }

        #endregion

        #region Stock Methods Tests

        [Fact]
        public async Task GetLowStockWarehouseAsync_ReturnsSuccessWithList()
        {
            // ARRANGE
            var medications = new List<Medication>
            {
                new Medication(Guid.NewGuid(), "Tableta", "Med1", "Company1", "BATCH001", "Sci1",
                    DateOnly.FromDateTime(DateTime.Now.AddYears(1)), 5, 20, 10, 5, 500, 100)
            };

            var medicationDtos = new List<MedicationDto>
            {
                new MedicationDto { QuantityWarehouse = 5, MinQuantityWarehouse = 10 }
            };

            _mockRepository.Setup(r => r.GetLowStockWarehouseAsync()).ReturnsAsync(medications);
            _mockMapper.Setup(m => m.Map<IEnumerable<MedicationDto>>(medications)).Returns(medicationDtos);

            // ACT
            var result = await _medicationService.GetLowStockWarehouseAsync();

            // ASSERT
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Single(result.Value);
        }

        [Fact]
        public async Task GetLowStockNurseAsync_ReturnsSuccessWithList()
        {
            // ARRANGE
            var medications = new List<Medication>
            {
                new Medication(Guid.NewGuid(), "Tableta", "Med1", "Company1", "BATCH001", "Sci1",
                    DateOnly.FromDateTime(DateTime.Now.AddYears(1)), 100, 2, 10, 5, 500, 100)
            };

            var medicationDtos = new List<MedicationDto>
            {
                new MedicationDto { QuantityNurse = 2, MinQuantityNurse = 5 }
            };

            _mockRepository.Setup(r => r.GetLowStockNurseAsync()).ReturnsAsync(medications);
            _mockMapper.Setup(m => m.Map<IEnumerable<MedicationDto>>(medications)).Returns(medicationDtos);

            // ACT
            var result = await _medicationService.GetLowStockNurseAsync();

            // ASSERT
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Single(result.Value);
        }

        [Fact]
        public async Task GetOverStockWarehouseAsync_ReturnsSuccessWithList()
        {
            // ARRANGE
            var medications = new List<Medication>
            {
                new Medication(Guid.NewGuid(), "Tableta", "Med1", "Company1", "BATCH001", "Sci1",
                    DateOnly.FromDateTime(DateTime.Now.AddYears(1)), 600, 20, 10, 5, 500, 100)
            };

            var medicationDtos = new List<MedicationDto>
            {
                new MedicationDto { QuantityWarehouse = 600, MaxQuantityWarehouse = 500 }
            };

            _mockRepository.Setup(r => r.GetOverstockWarehouseAsync()).ReturnsAsync(medications);
            _mockMapper.Setup(m => m.Map<IEnumerable<MedicationDto>>(medications)).Returns(medicationDtos);

            // ACT
            var result = await _medicationService.GetOverStockWarehouseAsync();

            // ASSERT
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Single(result.Value);
        }

        [Fact]
        public async Task GetOverStockNurseAsync_ReturnsSuccessWithList()
        {
            // ARRANGE
            var medications = new List<Medication>
            {
                new Medication(Guid.NewGuid(), "Tableta", "Med1", "Company1", "BATCH001", "Sci1",
                    DateOnly.FromDateTime(DateTime.Now.AddYears(1)), 100, 150, 10, 5, 500, 100)
            };

            var medicationDtos = new List<MedicationDto>
            {
                new MedicationDto { QuantityNurse = 150, MaxQuantityNurse = 100 }
            };

            _mockRepository.Setup(r => r.GetOverstockNurseAsync()).ReturnsAsync(medications);
            _mockMapper.Setup(m => m.Map<IEnumerable<MedicationDto>>(medications)).Returns(medicationDtos);

            // ACT
            var result = await _medicationService.GetOverStockNurseAsync();

            // ASSERT
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Single(result.Value);
        }

        #endregion
    }
}
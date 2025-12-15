using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Moq;
using AutoMapper;
using PolyclinicApplication.Services.Implementations;
using PolyclinicApplication.DTOs.Request.Derivations;
using PolyclinicApplication.DTOs.Response.Derivations;
using PolyclinicDomain.Entities;
using PolyclinicDomain.IRepositories;
using PolyclinicApplication.Common.Results;

namespace PolyclinicApplication.Tests.Services
{
    public class DerivationServiceTests
    {
        private readonly Mock<IDerivationRepository> _mockDerivationRepo;
        private readonly Mock<IPatientRepository> _mockPatientRepo;
        private readonly Mock<IDepartmentRepository> _mockDepartmentRepo;
        private readonly Mock<IMapper> _mockMapper;
        private readonly DerivationService _derivationService;

        public DerivationServiceTests()
        {
            _mockDerivationRepo = new Mock<IDerivationRepository>();
            _mockPatientRepo = new Mock<IPatientRepository>();
            _mockDepartmentRepo = new Mock<IDepartmentRepository>();
            _mockMapper = new Mock<IMapper>();
            _derivationService = new DerivationService(
                _mockDerivationRepo.Object,
                _mockPatientRepo.Object,
                _mockDepartmentRepo.Object,
                _mockMapper.Object
            );
        }

        #region CreateAsync Tests

        [Fact]
        public async Task CreateAsync_ValidDerivation_ReturnsSuccessResult()
        {
            // ARRANGE
            var createDto = new CreateDerivationDto
            {
                DepartmentFromId = Guid.NewGuid(),
                DepartmentToId = Guid.NewGuid(),
                PatientId = Guid.NewGuid(),
                DateTimeDer = DateTime.Now
            };

            var departmentFrom = new Department(createDto.DepartmentFromId, "Cardiología");
            var departmentTo = new Department(createDto.DepartmentToId, "Neurología");
            var patient = new Patient(createDto.PatientId, "Juan Pérez", "12345678", 30, "555-1234", "Calle 123");

            var derivation = new Derivation(
                Guid.NewGuid(),
                createDto.DepartmentFromId,
                createDto.DateTimeDer,
                createDto.PatientId,
                createDto.DepartmentToId
            );

            var derivationDto = new DerivationDto
            {
                DerivationId = derivation.DerivationId,
                DepartmentFromId = createDto.DepartmentFromId,
                DepartmentToId = createDto.DepartmentToId,
                PatientId = createDto.PatientId
            };

            _mockDepartmentRepo.Setup(r => r.GetByIdAsync(createDto.DepartmentFromId)).ReturnsAsync(departmentFrom);
            _mockDepartmentRepo.Setup(r => r.GetByIdAsync(createDto.DepartmentToId)).ReturnsAsync(departmentTo);
            _mockPatientRepo.Setup(r => r.GetByIdAsync(createDto.PatientId)).ReturnsAsync(patient);
            _mockDerivationRepo.Setup(r => r.AddAsync(It.IsAny<Derivation>())).ReturnsAsync(derivation);
            _mockMapper.Setup(m => m.Map<DerivationDto>(It.IsAny<Derivation>())).Returns(derivationDto);

            // ACT
            var result = await _derivationService.CreateAsync(createDto);

            // ASSERT
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(createDto.PatientId, result.Value.PatientId);
            _mockDerivationRepo.Verify(r => r.AddAsync(It.IsAny<Derivation>()), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_DepartmentFromNotFound_ReturnsFailureResult()
        {
            // ARRANGE
            var createDto = new CreateDerivationDto
            {
                DepartmentFromId = Guid.NewGuid(),
                DepartmentToId = Guid.NewGuid(),
                PatientId = Guid.NewGuid(),
                DateTimeDer = DateTime.Now
            };

            _mockDepartmentRepo.Setup(r => r.GetByIdAsync(createDto.DepartmentFromId)).ReturnsAsync((Department?)null);

            // ACT
            var result = await _derivationService.CreateAsync(createDto);

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("Departamento de origen no encontrado", result.ErrorMessage);
            _mockDerivationRepo.Verify(r => r.AddAsync(It.IsAny<Derivation>()), Times.Never);
        }

        [Fact]
        public async Task CreateAsync_DepartmentToNotFound_ReturnsFailureResult()
        {
            // ARRANGE
            var createDto = new CreateDerivationDto
            {
                DepartmentFromId = Guid.NewGuid(),
                DepartmentToId = Guid.NewGuid(),
                PatientId = Guid.NewGuid(),
                DateTimeDer = DateTime.Now
            };

            var departmentFrom = new Department(createDto.DepartmentFromId, "Cardiología");

            _mockDepartmentRepo.Setup(r => r.GetByIdAsync(createDto.DepartmentFromId)).ReturnsAsync(departmentFrom);
            _mockDepartmentRepo.Setup(r => r.GetByIdAsync(createDto.DepartmentToId)).ReturnsAsync((Department?)null);

            // ACT
            var result = await _derivationService.CreateAsync(createDto);

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("Departamento de destino no encontrado", result.ErrorMessage);
            _mockDerivationRepo.Verify(r => r.AddAsync(It.IsAny<Derivation>()), Times.Never);
        }

        [Fact]
        public async Task CreateAsync_PatientNotFound_ReturnsFailureResult()
        {
            // ARRANGE
            var createDto = new CreateDerivationDto
            {
                DepartmentFromId = Guid.NewGuid(),
                DepartmentToId = Guid.NewGuid(),
                PatientId = Guid.NewGuid(),
                DateTimeDer = DateTime.Now
            };

            var departmentFrom = new Department(createDto.DepartmentFromId, "Cardiología");
            var departmentTo = new Department(createDto.DepartmentToId, "Neurología");

            _mockDepartmentRepo.Setup(r => r.GetByIdAsync(createDto.DepartmentFromId)).ReturnsAsync(departmentFrom);
            _mockDepartmentRepo.Setup(r => r.GetByIdAsync(createDto.DepartmentToId)).ReturnsAsync(departmentTo);
            _mockPatientRepo.Setup(r => r.GetByIdAsync(createDto.PatientId)).ReturnsAsync((Patient?)null);

            // ACT
            var result = await _derivationService.CreateAsync(createDto);

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("Paciente no encontrado", result.ErrorMessage);
            _mockDerivationRepo.Verify(r => r.AddAsync(It.IsAny<Derivation>()), Times.Never);
        }

        [Fact]
        public async Task CreateAsync_RepositoryThrowsException_ReturnsFailureResult()
        {
            // ARRANGE
            var createDto = new CreateDerivationDto
            {
                DepartmentFromId = Guid.NewGuid(),
                DepartmentToId = Guid.NewGuid(),
                PatientId = Guid.NewGuid(),
                DateTimeDer = DateTime.Now
            };

            var departmentFrom = new Department(createDto.DepartmentFromId, "Cardiología");
            var departmentTo = new Department(createDto.DepartmentToId, "Neurología");
            var patient = new Patient(createDto.PatientId, "Juan Pérez", "12345678", 30, "555-1234", "Calle 123");

            _mockDepartmentRepo.Setup(r => r.GetByIdAsync(createDto.DepartmentFromId)).ReturnsAsync(departmentFrom);
            _mockDepartmentRepo.Setup(r => r.GetByIdAsync(createDto.DepartmentToId)).ReturnsAsync(departmentTo);
            _mockPatientRepo.Setup(r => r.GetByIdAsync(createDto.PatientId)).ReturnsAsync(patient);
            _mockDerivationRepo.Setup(r => r.AddAsync(It.IsAny<Derivation>())).ThrowsAsync(new Exception("Database error"));

            // ACT
            var result = await _derivationService.CreateAsync(createDto);

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("Error al guardar la derivación", result.ErrorMessage);
        }

        #endregion

        #region GetAllAsync Tests

        [Fact]
        public async Task GetAllAsync_DerivationsExist_ReturnsSuccessWithList()
        {
            // ARRANGE
            var derivations = new List<Derivation>
            {
                new Derivation(Guid.NewGuid(), Guid.NewGuid(), DateTime.Now, Guid.NewGuid(), Guid.NewGuid()),
                new Derivation(Guid.NewGuid(), Guid.NewGuid(), DateTime.Now, Guid.NewGuid(), Guid.NewGuid())
            };

            var derivationDtos = new List<DerivationDto>
            {
                new DerivationDto { DerivationId = derivations[0].DerivationId },
                new DerivationDto { DerivationId = derivations[1].DerivationId }
            };

            _mockDerivationRepo.Setup(r => r.GetAllWithIncludesAsync()).ReturnsAsync(derivations);
            _mockMapper.Setup(m => m.Map<IEnumerable<DerivationDto>>(derivations)).Returns(derivationDtos);

            // ACT
            var result = await _derivationService.GetAllAsync();

            // ASSERT
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(2, result.Value.Count());
            _mockDerivationRepo.Verify(r => r.GetAllWithIncludesAsync(), Times.Once);
        }

        [Fact]
        public async Task GetAllAsync_NoDerivations_ReturnsSuccessWithEmptyList()
        {
            // ARRANGE
            var emptyList = new List<Derivation>();
            _mockDerivationRepo.Setup(r => r.GetAllWithIncludesAsync()).ReturnsAsync(emptyList);
            _mockMapper.Setup(m => m.Map<IEnumerable<DerivationDto>>(emptyList)).Returns(new List<DerivationDto>());

            // ACT
            var result = await _derivationService.GetAllAsync();

            // ASSERT
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Empty(result.Value);
        }

        [Fact]
        public async Task GetAllAsync_RepositoryThrowsException_ReturnsFailureResult()
        {
            // ARRANGE
            _mockDerivationRepo.Setup(r => r.GetAllWithIncludesAsync()).ThrowsAsync(new Exception("Database error"));

            // ACT
            var result = await _derivationService.GetAllAsync();

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("Error al obtener las derivaciones", result.ErrorMessage);
        }

        #endregion

        #region GetByIdAsync Tests

        [Fact]
        public async Task GetByIdAsync_ValidId_ReturnsSuccessWithDerivation()
        {
            // ARRANGE
            var derivationId = Guid.NewGuid();
            var derivation = new Derivation(derivationId, Guid.NewGuid(), DateTime.Now, Guid.NewGuid(), Guid.NewGuid());
            var derivationDto = new DerivationDto { DerivationId = derivationId };

            _mockDerivationRepo.Setup(r => r.GetByIdWithIncludesAsync(derivationId)).ReturnsAsync(derivation);
            _mockMapper.Setup(m => m.Map<DerivationDto>(derivation)).Returns(derivationDto);

            // ACT
            var result = await _derivationService.GetByIdAsync(derivationId);

            // ASSERT
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(derivationId, result.Value.DerivationId);
            _mockDerivationRepo.Verify(r => r.GetByIdWithIncludesAsync(derivationId), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_DerivationNotFound_ReturnsFailureResult()
        {
            // ARRANGE
            var derivationId = Guid.NewGuid();
            _mockDerivationRepo.Setup(r => r.GetByIdWithIncludesAsync(derivationId)).ReturnsAsync((Derivation?)null);

            // ACT
            var result = await _derivationService.GetByIdAsync(derivationId);

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("Derivacion no encontrada", result.ErrorMessage);
        }

        [Fact]
        public async Task GetByIdAsync_RepositoryThrowsException_ReturnsFailureResult()
        {
            // ARRANGE
            var derivationId = Guid.NewGuid();
            _mockDerivationRepo.Setup(r => r.GetByIdWithIncludesAsync(derivationId)).ThrowsAsync(new Exception("Database error"));

            // ACT
            var result = await _derivationService.GetByIdAsync(derivationId);

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("Error al obtener la derivación", result.ErrorMessage);
        }

        #endregion

        #region DeleteAsync Tests

        [Fact]
        public async Task DeleteAsync_ValidId_ReturnsSuccessResult()
        {
            // ARRANGE
            var derivationId = Guid.NewGuid();
            var derivation = new Derivation(derivationId, Guid.NewGuid(), DateTime.Now, Guid.NewGuid(), Guid.NewGuid());

            _mockDerivationRepo.Setup(r => r.GetByIdAsync(derivationId)).ReturnsAsync(derivation);
            _mockDerivationRepo.Setup(r => r.DeleteByIdAsync(derivationId)).Returns(Task.CompletedTask);

            // ACT
            var result = await _derivationService.DeleteAsync(derivationId);

            // ASSERT
            Assert.True(result.IsSuccess);
            Assert.True(result.Value);
            _mockDerivationRepo.Verify(r => r.GetByIdAsync(derivationId), Times.Once);
            _mockDerivationRepo.Verify(r => r.DeleteByIdAsync(derivationId), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_DerivationNotFound_ReturnsFailureResult()
        {
            // ARRANGE
            var derivationId = Guid.NewGuid();
            _mockDerivationRepo.Setup(r => r.GetByIdAsync(derivationId)).ReturnsAsync((Derivation?)null);

            // ACT
            var result = await _derivationService.DeleteAsync(derivationId);

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("Derivacion no encontrada", result.ErrorMessage);
            _mockDerivationRepo.Verify(r => r.DeleteByIdAsync(It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public async Task DeleteAsync_RepositoryThrowsException_ReturnsFailureResult()
        {
            // ARRANGE
            var derivationId = Guid.NewGuid();
            var derivation = new Derivation(derivationId, Guid.NewGuid(), DateTime.Now, Guid.NewGuid(), Guid.NewGuid());

            _mockDerivationRepo.Setup(r => r.GetByIdAsync(derivationId)).ReturnsAsync(derivation);
            _mockDerivationRepo.Setup(r => r.DeleteByIdAsync(derivationId)).ThrowsAsync(new Exception("Database error"));

            // ACT
            var result = await _derivationService.DeleteAsync(derivationId);

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("Error al eliminar la derivación", result.ErrorMessage);
        }

        #endregion

        #region SearchByDepartmentFromNameAsync Tests

        [Fact]
        public async Task SearchByDepartmentFromNameAsync_DerivationsFound_ReturnsSuccessWithList()
        {
            // ARRANGE
            var departmentName = "Cardiología";
            var derivations = new List<Derivation>
            {
                new Derivation(Guid.NewGuid(), Guid.NewGuid(), DateTime.Now, Guid.NewGuid(), Guid.NewGuid())
            };

            var derivationDtos = new List<DerivationDto>
            {
                new DerivationDto { DepartmentFromName = departmentName }
            };

            _mockDerivationRepo.Setup(r => r.GetByDepartmentFromNameAsync(departmentName)).ReturnsAsync(derivations);
            _mockMapper.Setup(m => m.Map<IEnumerable<DerivationDto>>(derivations)).Returns(derivationDtos);

            // ACT
            var result = await _derivationService.SearchByDepartmentFromNameAsync(departmentName);

            // ASSERT
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Single(result.Value);
        }

        [Fact]
        public async Task SearchByDepartmentFromNameAsync_NoDerivationsFound_ReturnsFailureResult()
        {
            // ARRANGE
            var departmentName = "Nonexistent";
            var emptyList = new List<Derivation>();
            _mockDerivationRepo.Setup(r => r.GetByDepartmentFromNameAsync(departmentName)).ReturnsAsync(emptyList);

            // ACT
            var result = await _derivationService.SearchByDepartmentFromNameAsync(departmentName);

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("Derivacion no encontrada", result.ErrorMessage);
        }

        [Fact]
        public async Task SearchByDepartmentFromNameAsync_RepositoryThrowsException_ReturnsFailureResult()
        {
            // ARRANGE
            var departmentName = "Cardiología";
            _mockDerivationRepo.Setup(r => r.GetByDepartmentFromNameAsync(departmentName)).ThrowsAsync(new Exception("Database error"));

            // ACT
            var result = await _derivationService.SearchByDepartmentFromNameAsync(departmentName);

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("Error al buscar derivaciones", result.ErrorMessage);
        }

        #endregion

        #region SearchByDepartmentToNameAsync Tests

        [Fact]
        public async Task SearchByDepartmentToNameAsync_DerivationsFound_ReturnsSuccessWithList()
        {
            // ARRANGE
            var departmentName = "Neurología";
            var derivations = new List<Derivation>
            {
                new Derivation(Guid.NewGuid(), Guid.NewGuid(), DateTime.Now, Guid.NewGuid(), Guid.NewGuid())
            };

            var derivationDtos = new List<DerivationDto>
            {
                new DerivationDto { DepartmentToName = departmentName }
            };

            _mockDerivationRepo.Setup(r => r.GetByDepartmentToNameAsync(departmentName)).ReturnsAsync(derivations);
            _mockMapper.Setup(m => m.Map<IEnumerable<DerivationDto>>(derivations)).Returns(derivationDtos);

            // ACT
            var result = await _derivationService.SearchByDepartmentToNameAsync(departmentName);

            // ASSERT
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Single(result.Value);
        }

        [Fact]
        public async Task SearchByDepartmentToNameAsync_NoDerivationsFound_ReturnsFailureResult()
        {
            // ARRANGE
            var departmentName = "Nonexistent";
            var emptyList = new List<Derivation>();
            _mockDerivationRepo.Setup(r => r.GetByDepartmentToNameAsync(departmentName)).ReturnsAsync(emptyList);

            // ACT
            var result = await _derivationService.SearchByDepartmentToNameAsync(departmentName);

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("Derivacion no encontrada", result.ErrorMessage);
        }

        #endregion

        #region SearchByPatientNameAsync Tests

        [Fact]
        public async Task SearchByPatientNameAsync_DerivationsFound_ReturnsSuccessWithList()
        {
            // ARRANGE
            var patientName = "Juan Pérez";
            var derivations = new List<Derivation>
            {
                new Derivation(Guid.NewGuid(), Guid.NewGuid(), DateTime.Now, Guid.NewGuid(), Guid.NewGuid())
            };

            var derivationDtos = new List<DerivationDto>
            {
                new DerivationDto { PatientName = patientName }
            };

            _mockDerivationRepo.Setup(r => r.GetByPatientNameAsync(patientName)).ReturnsAsync(derivations);
            _mockMapper.Setup(m => m.Map<IEnumerable<DerivationDto>>(derivations)).Returns(derivationDtos);

            // ACT
            var result = await _derivationService.SearchByPatientNameAsync(patientName);

            // ASSERT
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Single(result.Value);
        }

        [Fact]
        public async Task SearchByPatientNameAsync_NoDerivationsFound_ReturnsFailureResult()
        {
            // ARRANGE
            var patientName = "Nonexistent";
            var emptyList = new List<Derivation>();
            _mockDerivationRepo.Setup(r => r.GetByPatientNameAsync(patientName)).ReturnsAsync(emptyList);

            // ACT
            var result = await _derivationService.SearchByPatientNameAsync(patientName);

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("Derivacion no encontrada", result.ErrorMessage);
        }

        #endregion

        #region SearchByDateAsync Tests

        [Fact]
        public async Task SearchByDateAsync_DerivationsFound_ReturnsSuccessWithList()
        {
            // ARRANGE
            var searchDate = DateTime.Now;
            var derivations = new List<Derivation>
            {
                new Derivation(Guid.NewGuid(), Guid.NewGuid(), searchDate, Guid.NewGuid(), Guid.NewGuid())
            };

            var derivationDtos = new List<DerivationDto>
            {
                new DerivationDto { DateTimeDer = searchDate }
            };

            _mockDerivationRepo.Setup(r => r.GetByDateAsync(searchDate.Date)).ReturnsAsync(derivations);
            _mockMapper.Setup(m => m.Map<IEnumerable<DerivationDto>>(derivations)).Returns(derivationDtos);

            // ACT
            var result = await _derivationService.SearchByDateAsync(searchDate);

            // ASSERT
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Single(result.Value);
        }

        [Fact]
        public async Task SearchByDateAsync_NoDerivationsFound_ReturnsFailureResult()
        {
            // ARRANGE
            var searchDate = DateTime.Now;
            var emptyList = new List<Derivation>();
            _mockDerivationRepo.Setup(r => r.GetByDateAsync(searchDate.Date)).ReturnsAsync(emptyList);

            // ACT
            var result = await _derivationService.SearchByDateAsync(searchDate);

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("Derivacion no encontrada", result.ErrorMessage);
        }

        #endregion

        #region SearchByPatientIdentificationAsync Tests

        [Fact]
        public async Task SearchByPatientIdentificationAsync_DerivationsFound_ReturnsSuccessWithList()
        {
            // ARRANGE
            var patientIdentification = "12345678";
            var derivations = new List<Derivation>
            {
                new Derivation(Guid.NewGuid(), Guid.NewGuid(), DateTime.Now, Guid.NewGuid(), Guid.NewGuid())
            };

            var derivationDtos = new List<DerivationDto>
            {
                new DerivationDto { PatientIdentification = patientIdentification }
            };

            _mockDerivationRepo.Setup(r => r.GetByPatientIdentificationAsync(patientIdentification)).ReturnsAsync(derivations);
            _mockMapper.Setup(m => m.Map<IEnumerable<DerivationDto>>(derivations)).Returns(derivationDtos);

            // ACT
            var result = await _derivationService.SearchByPatientIdentificationAsync(patientIdentification);

            // ASSERT
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Single(result.Value);
        }

        [Fact]
        public async Task SearchByPatientIdentificationAsync_NoDerivationsFound_ReturnsFailureResult()
        {
            // ARRANGE
            var patientIdentification = "99999999";
            var emptyList = new List<Derivation>();
            _mockDerivationRepo.Setup(r => r.GetByPatientIdentificationAsync(patientIdentification)).ReturnsAsync(emptyList);

            // ACT
            var result = await _derivationService.SearchByPatientIdentificationAsync(patientIdentification);

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("Derivacion no encontrada", result.ErrorMessage);
        }

        #endregion
    }
}
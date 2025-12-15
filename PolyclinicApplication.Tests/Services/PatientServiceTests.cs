using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Moq;
using AutoMapper;
using PolyclinicApplication.Services.Implementations;
using PolyclinicApplication.DTOs.Request.Patients;
using PolyclinicApplication.DTOs.Response.Patients;
using PolyclinicDomain.Entities;
using PolyclinicDomain.IRepositories;
using PolyclinicApplication.Common.Results;

namespace PolyclinicApplication.Tests.Services
{
    public class PatientServiceTests
    {
        private readonly Mock<IPatientRepository> _mockRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly PatientService _patientService;

        public PatientServiceTests()
        {
            _mockRepository = new Mock<IPatientRepository>();
            _mockMapper = new Mock<IMapper>();
            _patientService = new PatientService(_mockRepository.Object, _mockMapper.Object);
        }

        #region CreateAsync Tests

        [Fact]
        public async Task CreateAsync_ValidPatient_ReturnsSuccessResult()
        {
            // ARRANGE
            var createDto = new CreatePatientDto
            {
                Name = "Juan Pérez",
                Identification = "12345678",
                Age = 30,
                Contact = "555-1234",
                Address = "Calle 123"
            };

            var patient = new Patient(
                Guid.NewGuid(),
                createDto.Name,
                createDto.Identification,
                createDto.Age,
                createDto.Contact,
                createDto.Address
            );

            var patientDto = new PatientDto
            {
                PatientId = patient.PatientId,
                Name = patient.Name,
                Identification = patient.Identification
            };

            _mockRepository.Setup(r => r.GetByIdentificationAsync(createDto.Identification)).ReturnsAsync((Patient?)null);
            _mockRepository.Setup(r => r.AddAsync(It.IsAny<Patient>())).ReturnsAsync(patient);
            _mockMapper.Setup(m => m.Map<PatientDto>(It.IsAny<Patient>())).Returns(patientDto);

            // ACT
            var result = await _patientService.CreateAsync(createDto);

            // ASSERT
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(createDto.Name, result.Value.Name);
            Assert.Equal(createDto.Identification, result.Value.Identification);
            _mockRepository.Verify(r => r.AddAsync(It.IsAny<Patient>()), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_DuplicateIdentification_ReturnsFailureResult()
        {
            // ARRANGE
            var createDto = new CreatePatientDto
            {
                Identification = "12345678",
                Name = "Test",
                Age = 30,
                Contact = "555-1234",
                Address = "Calle 123"
            };

            var existingPatient = new Patient(
                Guid.NewGuid(),
                "Existing Patient",
                createDto.Identification,
                25,
                "555-5678",
                "Otra calle"
            );

            _mockRepository.Setup(r => r.GetByIdentificationAsync(createDto.Identification)).ReturnsAsync(existingPatient);

            // ACT
            var result = await _patientService.CreateAsync(createDto);

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("Ya existe un paciente con esta identificación", result.ErrorMessage);
            _mockRepository.Verify(r => r.AddAsync(It.IsAny<Patient>()), Times.Never);
        }

        [Fact]
        public async Task CreateAsync_RepositoryThrowsException_ReturnsFailureResult()
        {
            // ARRANGE
            var createDto = new CreatePatientDto
            {
                Identification = "12345678",
                Name = "Test",
                Age = 30,
                Contact = "555-1234",
                Address = "Calle 123"
            };

            _mockRepository.Setup(r => r.GetByIdentificationAsync(createDto.Identification)).ReturnsAsync((Patient?)null);
            _mockRepository.Setup(r => r.AddAsync(It.IsAny<Patient>())).ThrowsAsync(new Exception("Database error"));

            // ACT
            var result = await _patientService.CreateAsync(createDto);

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("Error al guardar el paciente", result.ErrorMessage);
        }

        #endregion

        #region GetAllAsync Tests

        [Fact]
        public async Task GetAllAsync_PatientsExist_ReturnsSuccessWithList()
        {
            // ARRANGE
            var patients = new List<Patient>
            {
                new Patient(Guid.NewGuid(), "Juan Pérez", "12345678", 30, "555-1234", "Calle 123"),
                new Patient(Guid.NewGuid(), "María García", "87654321", 25, "555-5678", "Calle 456")
            };

            var patientDtos = new List<PatientDto>
            {
                new PatientDto { PatientId = patients[0].PatientId, Name = "Juan Pérez" },
                new PatientDto { PatientId = patients[1].PatientId, Name = "María García" }
            };

            _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(patients);
            _mockMapper.Setup(m => m.Map<IEnumerable<PatientDto>>(patients)).Returns(patientDtos);

            // ACT
            var result = await _patientService.GetAllAsync();

            // ASSERT
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(2, result.Value.Count());
            _mockRepository.Verify(r => r.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetAllAsync_NoPatients_ReturnsSuccessWithEmptyList()
        {
            // ARRANGE
            var emptyList = new List<Patient>();
            _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(emptyList);
            _mockMapper.Setup(m => m.Map<IEnumerable<PatientDto>>(emptyList)).Returns(new List<PatientDto>());

            // ACT
            var result = await _patientService.GetAllAsync();

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
            var result = await _patientService.GetAllAsync();

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("Error al obtener pacientes", result.ErrorMessage);
        }

        #endregion

        #region GetByIdAsync Tests

        [Fact]
        public async Task GetByIdAsync_ValidId_ReturnsSuccessWithPatient()
        {
            // ARRANGE
            var patientId = Guid.NewGuid();
            var patient = new Patient(patientId, "Juan Pérez", "12345678", 30, "555-1234", "Calle 123");
            var patientDto = new PatientDto { PatientId = patientId, Name = "Juan Pérez" };

            _mockRepository.Setup(r => r.GetByIdAsync(patientId)).ReturnsAsync(patient);
            _mockMapper.Setup(m => m.Map<PatientDto>(patient)).Returns(patientDto);

            // ACT
            var result = await _patientService.GetByIdAsync(patientId);

            // ASSERT
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(patientId, result.Value.PatientId);
            Assert.Equal("Juan Pérez", result.Value.Name);
            _mockRepository.Verify(r => r.GetByIdAsync(patientId), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_PatientNotFound_ReturnsFailureResult()
        {
            // ARRANGE
            var patientId = Guid.NewGuid();
            _mockRepository.Setup(r => r.GetByIdAsync(patientId)).ReturnsAsync((Patient?)null);

            // ACT
            var result = await _patientService.GetByIdAsync(patientId);

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("Paciente no encontrado", result.ErrorMessage);
        }

        [Fact]
        public async Task GetByIdAsync_RepositoryThrowsException_ReturnsFailureResult()
        {
            // ARRANGE
            var patientId = Guid.NewGuid();
            _mockRepository.Setup(r => r.GetByIdAsync(patientId)).ThrowsAsync(new Exception("Database error"));

            // ACT
            var result = await _patientService.GetByIdAsync(patientId);

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("Error al obtener paciente", result.ErrorMessage);
        }

        #endregion

        #region GetByNameAsync Tests

        [Fact]
        public async Task GetByNameAsync_PatientsFound_ReturnsSuccessWithList()
        {
            // ARRANGE
            var name = "Juan";
            var patients = new List<Patient>
            {
                new Patient(Guid.NewGuid(), "Juan Pérez", "12345678", 30, "555-1234", "Calle 123"),
                new Patient(Guid.NewGuid(), "Juan García", "87654321", 25, "555-5678", "Calle 456")
            };

            var patientDtos = new List<PatientDto>
            {
                new PatientDto { Name = "Juan Pérez" },
                new PatientDto { Name = "Juan García" }
            };

            _mockRepository.Setup(r => r.GetByNameAsync(name)).ReturnsAsync(patients);
            _mockMapper.Setup(m => m.Map<IEnumerable<PatientDto>>(patients)).Returns(patientDtos);

            // ACT
            var result = await _patientService.GetByNameAsync(name);

            // ASSERT
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(2, result.Value.Count());
        }

        [Fact]
        public async Task GetByNameAsync_NoPatientsFound_ReturnsFailureResult()
        {
            // ARRANGE
            var name = "Nonexistent";
            var emptyList = new List<Patient>();
            _mockRepository.Setup(r => r.GetByNameAsync(name)).ReturnsAsync(emptyList);

            // ACT
            var result = await _patientService.GetByNameAsync(name);

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("Paciente no encontrado", result.ErrorMessage);
        }

        [Fact]
        public async Task GetByNameAsync_RepositoryThrowsException_ReturnsFailureResult()
        {
            // ARRANGE
            var name = "Test";
            _mockRepository.Setup(r => r.GetByNameAsync(name)).ThrowsAsync(new Exception("Database error"));

            // ACT
            var result = await _patientService.GetByNameAsync(name);

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("Error al buscar paciente por nombre", result.ErrorMessage);
        }

        #endregion

        #region GetByIdentificationAsync Tests

        [Fact]
        public async Task GetByIdentificationAsync_PatientFound_ReturnsSuccessWithPatient()
        {
            // ARRANGE
            var identification = "12345678";
            var patient = new Patient(Guid.NewGuid(), "Juan Pérez", identification, 30, "555-1234", "Calle 123");
            var patientDto = new PatientDto { Identification = identification, Name = "Juan Pérez" };

            _mockRepository.Setup(r => r.GetByIdentificationAsync(identification)).ReturnsAsync(patient);
            _mockMapper.Setup(m => m.Map<PatientDto>(patient)).Returns(patientDto);

            // ACT
            var result = await _patientService.GetByIdentificationAsync(identification);

            // ASSERT
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(identification, result.Value.Identification);
        }

        [Fact]
        public async Task GetByIdentificationAsync_PatientNotFound_ReturnsFailureResult()
        {
            // ARRANGE
            var identification = "99999999";
            _mockRepository.Setup(r => r.GetByIdentificationAsync(identification)).ReturnsAsync((Patient?)null);

            // ACT
            var result = await _patientService.GetByIdentificationAsync(identification);

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("Paciente no encontrado", result.ErrorMessage);
        }

        [Fact]
        public async Task GetByIdentificationAsync_RepositoryThrowsException_ReturnsFailureResult()
        {
            // ARRANGE
            var identification = "12345678";
            _mockRepository.Setup(r => r.GetByIdentificationAsync(identification)).ThrowsAsync(new Exception("Database error"));

            // ACT
            var result = await _patientService.GetByIdentificationAsync(identification);

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("Error al buscar paciente por identificación", result.ErrorMessage);
        }

        #endregion

        #region GetByAgeAsync Tests

        [Fact]
        public async Task GetByAgeAsync_PatientsFound_ReturnsSuccessWithList()
        {
            // ARRANGE
            var age = 30;
            var patients = new List<Patient>
            {
                new Patient(Guid.NewGuid(), "Juan Pérez", "12345678", age, "555-1234", "Calle 123"),
                new Patient(Guid.NewGuid(), "María García", "87654321", age, "555-5678", "Calle 456")
            };

            var patientDtos = new List<PatientDto>
            {
                new PatientDto { Age = age, Name = "Juan Pérez" },
                new PatientDto { Age = age, Name = "María García" }
            };

            _mockRepository.Setup(r => r.GetByAgeAsync(age)).ReturnsAsync(patients);
            _mockMapper.Setup(m => m.Map<IEnumerable<PatientDto>>(patients)).Returns(patientDtos);

            // ACT
            var result = await _patientService.GetByAgeAsync(age);

            // ASSERT
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(2, result.Value.Count());
        }

        [Fact]
        public async Task GetByAgeAsync_NoPatientsFound_ReturnsFailureResult()
        {
            // ARRANGE
            var age = 999;
            var emptyList = new List<Patient>();
            _mockRepository.Setup(r => r.GetByAgeAsync(age)).ReturnsAsync(emptyList);

            // ACT
            var result = await _patientService.GetByAgeAsync(age);

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("Paciente no encontrado", result.ErrorMessage);
        }

        [Fact]
        public async Task GetByAgeAsync_RepositoryThrowsException_ReturnsFailureResult()
        {
            // ARRANGE
            var age = 30;
            _mockRepository.Setup(r => r.GetByAgeAsync(age)).ThrowsAsync(new Exception("Database error"));

            // ACT
            var result = await _patientService.GetByAgeAsync(age);

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("Error al buscar paciente por edad", result.ErrorMessage);
        }

        #endregion

        #region UpdateAsync Tests

        [Fact]
        public async Task UpdateAsync_ValidUpdate_ReturnsSuccessResult()
        {
            // ARRANGE
            var patientId = Guid.NewGuid();
            var existingPatient = new Patient(patientId, "Juan Pérez", "12345678", 30, "555-1234", "Calle 123");
            var updateDto = new UpdatePatientDto
            {
                Name = "Juan Pérez Actualizado",
                Identification = "87654321",
                Age = 31,
                Contact = "555-9999",
                Address = "Nueva Calle 456"
            };

            _mockRepository.Setup(r => r.GetByIdAsync(patientId)).ReturnsAsync(existingPatient);
            _mockRepository.Setup(r => r.GetByIdentificationAsync(updateDto.Identification)).ReturnsAsync((Patient?)null);
            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Patient>())).Returns(Task.CompletedTask);

            // ACT
            var result = await _patientService.UpdateAsync(patientId, updateDto);

            // ASSERT
            Assert.True(result.IsSuccess);
            Assert.True(result.Value);
            _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Patient>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_PatientNotFound_ReturnsFailureResult()
        {
            // ARRANGE
            var patientId = Guid.NewGuid();
            var updateDto = new UpdatePatientDto { Name = "New Name" };
            _mockRepository.Setup(r => r.GetByIdAsync(patientId)).ReturnsAsync((Patient?)null);

            // ACT
            var result = await _patientService.UpdateAsync(patientId, updateDto);

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("Paciente no encontrado", result.ErrorMessage);
            _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Patient>()), Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_DuplicateIdentification_ReturnsFailureResult()
        {
            // ARRANGE
            var patientId = Guid.NewGuid();
            var existingPatient = new Patient(patientId, "Juan Pérez", "12345678", 30, "555-1234", "Calle 123");
            var otherPatient = new Patient(Guid.NewGuid(), "Otro Paciente", "87654321", 25, "555-5678", "Otra calle");
            var updateDto = new UpdatePatientDto { Identification = "87654321" };

            _mockRepository.Setup(r => r.GetByIdAsync(patientId)).ReturnsAsync(existingPatient);
            _mockRepository.Setup(r => r.GetByIdentificationAsync(updateDto.Identification)).ReturnsAsync(otherPatient);

            // ACT
            var result = await _patientService.UpdateAsync(patientId, updateDto);

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("Ya existe un paciente con esta identificación", result.ErrorMessage);
            _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Patient>()), Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_RepositoryThrowsException_ReturnsFailureResult()
        {
            // ARRANGE
            var patientId = Guid.NewGuid();
            var existingPatient = new Patient(patientId, "Juan Pérez", "12345678", 30, "555-1234", "Calle 123");
            var updateDto = new UpdatePatientDto { Name = "New Name" };

            _mockRepository.Setup(r => r.GetByIdAsync(patientId)).ReturnsAsync(existingPatient);
            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Patient>())).ThrowsAsync(new Exception("Database error"));

            // ACT
            var result = await _patientService.UpdateAsync(patientId, updateDto);

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("Error al actualizar el paciente", result.ErrorMessage);
        }

        #endregion

        #region DeleteAsync Tests

        [Fact]
        public async Task DeleteAsync_ValidId_ReturnsSuccessResult()
        {
            // ARRANGE
            var patientId = Guid.NewGuid();
            var patient = new Patient(patientId, "Juan Pérez", "12345678", 30, "555-1234", "Calle 123");

            _mockRepository.Setup(r => r.GetByIdAsync(patientId)).ReturnsAsync(patient);
            _mockRepository.Setup(r => r.DeleteByIdAsync(patientId)).Returns(Task.CompletedTask);

            // ACT
            var result = await _patientService.DeleteAsync(patientId);

            // ASSERT
            Assert.True(result.IsSuccess);
            Assert.True(result.Value);
            _mockRepository.Verify(r => r.GetByIdAsync(patientId), Times.Once);
            _mockRepository.Verify(r => r.DeleteByIdAsync(patientId), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_PatientNotFound_ReturnsFailureResult()
        {
            // ARRANGE
            var patientId = Guid.NewGuid();
            _mockRepository.Setup(r => r.GetByIdAsync(patientId)).ReturnsAsync((Patient?)null);

            // ACT
            var result = await _patientService.DeleteAsync(patientId);

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("Paciente no encontrado", result.ErrorMessage);
            _mockRepository.Verify(r => r.DeleteByIdAsync(It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public async Task DeleteAsync_RepositoryThrowsException_ReturnsFailureResult()
        {
            // ARRANGE
            var patientId = Guid.NewGuid();
            var patient = new Patient(patientId, "Juan Pérez", "12345678", 30, "555-1234", "Calle 123");

            _mockRepository.Setup(r => r.GetByIdAsync(patientId)).ReturnsAsync(patient);
            _mockRepository.Setup(r => r.DeleteByIdAsync(patientId)).ThrowsAsync(new Exception("Database error"));

            // ACT
            var result = await _patientService.DeleteAsync(patientId);

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("Error al eliminar el paciente", result.ErrorMessage);
        }

        #endregion
    }
}